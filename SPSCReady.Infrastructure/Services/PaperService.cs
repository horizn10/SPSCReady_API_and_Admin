using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SPSCReady.Infrastructure.Services
{
    public class PaperService : IPaperService
    {
        private readonly ApplicationDbContext _context;

        public PaperService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<PostDto>> GetPostsAsync(int departmentId)
        {
            return await _context.Posts
                .Where(p => p.DepartmentId == departmentId)
                .Select(p => new PostDto { Id = p.Id, Name = p.Name, DepartmentId = p.DepartmentId })
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<ExamStageDto>> GetExamStagesAsync()
        {
            return await _context.ExamStages
                .Select(es => new ExamStageDto { Id = es.Id, Name = es.Name })
                .OrderBy(es => es.Name)
                .ToListAsync();
        }

        public async Task<List<ExamSubjectDto>> GetSubjectsAsync()
        {
            return await _context.Subjects
                .Select(s => new ExamSubjectDto { Id = s.Id, Name = s.Name })
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<List<ExamPaperListDto>> GetPapersAsync(
            string? search = null,
            int? stageId = null,
            string? departmentName = null,
            int? examYear = null,
            string? stageName = null,
            string? postName = null)
        {
            // IMPORTANT: Query ExamPaperSubjects directly to ensure ONE row per PDF/Subject
            var query = _context.ExamPaperSubjects
                .Include(s => s.ExamPaper).ThenInclude(ep => ep.Departments).ThenInclude(d => d.Department)
                .Include(s => s.ExamPaper).ThenInclude(ep => ep.Posts).ThenInclude(p => p.Post)
                .Include(s => s.Stage)
                .Include(s => s.Subject)
                .AsNoTracking();

            // Filters applied to the parent ExamPaper or the specific Subject record
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.ExamPaper.Title.Contains(search) || (s.SubjectName != null && s.SubjectName.Contains(search)));
            }

            if (stageId.HasValue)
            {
                query = query.Where(s => s.StageId == stageId.Value);
            }

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                query = query.Where(s => s.ExamPaper.Departments.Any(d => d.Department.Name.Contains(departmentName)));
            }

            if (examYear.HasValue)
            {
                query = query.Where(s =>
                    (s.ExamPaper.ExamDate.HasValue && s.ExamPaper.ExamDate.Value.Year == examYear.Value) ||
                    (!s.ExamPaper.ExamDate.HasValue && s.ExamPaper.UploadedAt.Year == examYear.Value));
            }

            if (!string.IsNullOrWhiteSpace(stageName))
            {
                query = query.Where(s => s.Stage.Name.Contains(stageName));
            }

            if (!string.IsNullOrWhiteSpace(postName))
            {
                query = query.Where(s => s.ExamPaper.Posts.Any(p => p.Post.Name.Contains(postName)));
            }

            var paperList = await query
                .OrderByDescending(s => s.Date ?? s.ExamPaper.UploadedAt)
                .Select(s => new ExamPaperListDto
                {
                    Id = s.Id, // The unique ID of the subject entry
                    Title = s.ExamPaper.Title, // e.g., "Paper - I"
                    Description = s.ExamPaper.Description,
                    ExamDate = s.ExamPaper.ExamDate ?? s.ExamPaper.UploadedAt,
                    UploadedAt = s.ExamPaper.UploadedAt,
                    DepartmentNames = s.ExamPaper.Departments.Select(d => d.Department.Name).ToList(),
                    PostNames = s.ExamPaper.Posts.Select(p => p.Post.Name).ToList(),

                    // Specific singular info for this card
                    SubjectName = s.SubjectName ?? s.Subject.Name,
                    StageName = s.Stage.Name,
                    Url = s.Url,

                    // Combined Exam Name for context
                    ExamName = (s.ExamPaper.Posts.Select(p => p.Post.Name).FirstOrDefault() ?? "SPSC Exam") +
                               " (" + (s.ExamPaper.ExamDate.HasValue ? s.ExamPaper.ExamDate.Value.Year : s.ExamPaper.UploadedAt.Year) + ")"
                })
                .ToListAsync();

            return paperList;
        }

        public async Task<bool> UploadPaperAsync(IFormFile pdfFile, UploadPaperDto request, string webRootPath)
        {
            try
            {
                if (pdfFile == null || pdfFile.Length == 0 || !pdfFile.ContentType.Equals("application/pdf"))
                    return false;

                var pdfsDir = Path.Combine(webRootPath, "pdfs");
                Directory.CreateDirectory(pdfsDir);
                var fileName = $"{Guid.NewGuid():N}.pdf";
                var filePath = Path.Combine(pdfsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                var examPaper = new ExamPaper
                {
                    Title = request.Title,
                    UploadedBy = "system",
                    UploadedAt = DateTime.UtcNow,
                    ExamDate = DateTime.UtcNow
                };

                examPaper.Departments.Add(new ExamPaperDept { DepartmentId = request.DepartmentId });
                examPaper.Posts.Add(new ExamPaperPost { PostId = request.PostId });
                examPaper.Stages.Add(new ExamPaperStage { StageId = request.StageId });

                var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId);
                var subjectLeaf = new ExamPaperSubject
                {
                    StageId = request.StageId,
                    SubjectId = request.SubjectId,
                    SubjectName = subject?.Name,
                    Url = $"/pdfs/{fileName}",
                    Date = DateTime.UtcNow
                };
                examPaper.Subjects.Add(subjectLeaf);

                _context.ExamPapers.Add(examPaper);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
