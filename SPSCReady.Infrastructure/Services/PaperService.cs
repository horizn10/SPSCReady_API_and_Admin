// TEMPORARY DEBUG VERSION — revert after finding the error
// SPSCReady.Infrastructure/Services/PaperService.cs

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
                query = query.Where(ep => ep.Title.Contains(search));

            if (stageId.HasValue)
                query = query.Where(ep => ep.Stages.Any(s => s.StageId == stageId.Value) || ep.Subjects.Any(s => s.StageId == stageId.Value));

            if (!string.IsNullOrWhiteSpace(departmentName))
                query = query.Where(ep => ep.Departments.Any(d => d.Department.Name.Contains(departmentName)));

            if (examYear.HasValue)
                query = query.Where(ep => ep.ExamDate.HasValue && ep.ExamDate.Value.Year == examYear.Value);

            if (!string.IsNullOrWhiteSpace(stageName))
                query = query.Where(ep => ep.Stages.Any(s => s.Stage.Name.Contains(stageName)));

            if (!string.IsNullOrWhiteSpace(postName))
                query = query.Where(ep => ep.Posts.Any(p => p.Post.Name.Contains(postName)));

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
            catch (Exception ex)
            {
                // ✅ TEMPORARY: throw so we can see the real error
                throw new Exception($"UploadPaperAsync failed: {ex.Message}", ex);
            }
        }

        public async Task<bool> MultiUploadPaperAsync(
            List<IFormFile> pdfFiles,
            MultiUploadPaperDto request,
            string webRootPath)
        {
            // ── STEP 1: Log incoming values so we can verify what arrived ─────────
            Console.WriteLine($"[MultiUpload] webRootPath = '{webRootPath}'");
            Console.WriteLine($"[MultiUpload] Title       = '{request.Title}'");
            Console.WriteLine($"[MultiUpload] DeptId      = {request.DepartmentId}");
            Console.WriteLine($"[MultiUpload] PostId      = {request.PostId}");
            Console.WriteLine($"[MultiUpload] ExamYear    = {request.ExamYear}");
            Console.WriteLine($"[MultiUpload] PdfFiles    = {pdfFiles?.Count ?? 0}");
            Console.WriteLine($"[MultiUpload] Papers      = {request.SubjectPapers?.Count ?? 0}");

            // ── STEP 2: Null / empty guards — log exactly which check fails ───────
            if (pdfFiles == null || pdfFiles.Count == 0)
                throw new Exception("pdfFiles is null or empty.");

            foreach (var f in pdfFiles)
            {
                Console.WriteLine($"[MultiUpload] File: '{f.FileName}', Length={f.Length}, ContentType='{f.ContentType}'");
                if (f.Length == 0)
                    throw new Exception($"File '{f.FileName}' has zero length.");
                if (!f.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"File '{f.FileName}' has wrong ContentType '{f.ContentType}'. Expected 'application/pdf'.");
            }

            if (request.SubjectPapers == null || request.SubjectPapers.Count == 0)
                throw new Exception("SubjectPapers list is null or empty.");

            if (request.SubjectPapers.Count != pdfFiles.Count)
                throw new Exception($"Count mismatch: SubjectPapers={request.SubjectPapers.Count}, PdfFiles={pdfFiles.Count}.");

            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new Exception("webRootPath is null or empty — IWebHostEnvironment.WebRootPath is not set. Make sure wwwroot exists in the API project.");

            // ── STEP 3: Attempt the actual work — throw on any error ──────────────
            try
            {
                var pdfsDir = Path.Combine(webRootPath, "pdfs");
                Console.WriteLine($"[MultiUpload] pdfsDir = '{pdfsDir}'");
                Directory.CreateDirectory(pdfsDir);

                // ✅ FIX: Create ExamPaper without junction entities first
                var examPaper = new ExamPaper
                {
                    Title = request.Title,
                    ExamDate = new DateTime(request.ExamYear, 1, 1),
                    UploadedBy = "system",
                    UploadedAt = DateTime.UtcNow
                };

                // ✅ FIX: Save first to get ExamPaper.Id
                _context.ExamPapers.Add(examPaper);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[MultiUpload] Saved initial ExamPaper with Id={examPaper.Id}");

                // ✅ FIX: Now add junction entities with explicit ExamId
                examPaper.Departments.Add(new ExamPaperDept 
                { 
                    ExamId = examPaper.Id,
                    DepartmentId = request.DepartmentId 
                });
                examPaper.Posts.Add(new ExamPaperPost 
                { 
                    ExamId = examPaper.Id,
                    PostId = request.PostId 
                });

                foreach (var stageId in request.SubjectPapers.Select(sp => sp.StageId).Distinct())
                {
                    examPaper.Stages.Add(new ExamPaperStage 
                    { 
                        ExamId = examPaper.Id,
                        StageId = stageId 
                    });
                }

                // ✅ FIX: Save junction entities
                await _context.SaveChangesAsync();
                Console.WriteLine($"[MultiUpload] Saved departments, posts, and stages");

                for (int i = 0; i < request.SubjectPapers.Count; i++)
                {
                    var pdfFile = pdfFiles[i];
                    var subjectPaper = request.SubjectPapers[i];

                    var fileName = $"{Guid.NewGuid():N}.pdf";
                    var filePath = Path.Combine(pdfsDir, fileName);
                    Console.WriteLine($"[MultiUpload] Saving file to '{filePath}'");

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(stream);
                    }

                    var subjectName = (subjectPaper.Title ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(subjectName))
                        throw new Exception($"SubjectPaper[{i}].Title is empty.");

                    var subject = await _context.Subjects
                        .FirstOrDefaultAsync(s => s.Name != null &&
                            s.Name.ToLower() == subjectName.ToLower() &&
                            s.StageId == subjectPaper.StageId);

                    if (subject == null)
                    {
                        subject = new Subject
                        {
                            Name = subjectName,
                            StageId = subjectPaper.StageId
                        };
                        _context.Subjects.Add(subject);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[MultiUpload] Created new Subject Id={subject.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"[MultiUpload] Reusing Subject Id={subject.Id}");
                    }

                    // ✅ FIX: Explicitly set ExamId
                    examPaper.Subjects.Add(new ExamPaperSubject
                    {
                        ExamId = examPaper.Id,
                        StageId = subjectPaper.StageId,
                        SubjectId = subject.Id,
                        SubjectName = subject.Name,
                        Url = $"/pdfs/{fileName}",
                        Date = DateTime.UtcNow
                    });
                }

                // ✅ FIX: Final save for subjects
                await _context.SaveChangesAsync();
                Console.WriteLine($"[MultiUpload] ✅ Success. ExamPaper.Id = {examPaper.Id} with {examPaper.Subjects.Count} subjects");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MultiUpload] ❌ ERROR: {ex}");
                throw new Exception($"MultiUploadPaperAsync failed: {ex.Message}", ex);
            }
        }
    }
}