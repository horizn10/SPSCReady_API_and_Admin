using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using System;
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
            var query = _context.ExamPapers
                .Include(ep => ep.Departments).ThenInclude(d => d.Department)
                .Include(ep => ep.Posts).ThenInclude(p => p.Post)
                .Include(ep => ep.Stages).ThenInclude(s => s.Stage)
                .Include(ep => ep.Subjects).ThenInclude(s => s.Subject).ThenInclude(sub => sub.Stage)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(ep => ep.Title.Contains(search));
            }

            if (stageId.HasValue)
            {
                query = query.Where(ep => ep.Stages.Any(s => s.StageId == stageId.Value) || ep.Subjects.Any(s => s.StageId == stageId.Value));
            }

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                query = query.Where(ep => ep.Departments.Any(d => d.Department.Name.Contains(departmentName)));
            }

            if (examYear.HasValue)
            {
                query = query.Where(ep => ep.ExamDate.HasValue && ep.ExamDate.Value.Year == examYear.Value);
            }

            if (!string.IsNullOrWhiteSpace(stageName))
            {
                query = query.Where(ep => ep.Stages.Any(s => s.Stage.Name.Contains(stageName)));
            }

            if (!string.IsNullOrWhiteSpace(postName))
            {
                query = query.Where(ep => ep.Posts.Any(p => p.Post.Name.Contains(postName)));
            }

            var papers = await query
                .OrderByDescending(ep => ep.UploadedAt)
                .Select(ep => new ExamPaperListDto
                {
                    Id = ep.Id,
                    Title = ep.Title,
                    Description = ep.Description,
                    ExamDate = ep.ExamDate,
                    UploadedAt = ep.UploadedAt,
                    DepartmentNames = ep.Departments.Select(d => d.Department.Name).ToList(),
                    PostNames = ep.Posts.Select(p => p.Post.Name).ToList(),
                    StageNames = ep.Stages.Select(s => s.Stage.Name).ToList(),
                    SubjectNames = ep.Subjects.Select(sub => sub.Subject.Name).ToList()
                })
                .ToListAsync();

            return papers;
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
                    UploadedAt = DateTime.UtcNow
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
