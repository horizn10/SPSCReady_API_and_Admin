// TEMPORARY DEBUG VERSION — revert after finding the error
// SPSCReady.Infrastructure/Services/PaperService.cs

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
        private readonly IR2StorageService _r2Storage;

        public PaperService(ApplicationDbContext context, IR2StorageService r2Storage)
        {
            _context = context;
            _r2Storage = r2Storage;
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

        public async Task<string?> GetPdfUrlAsync(int subjectEntryId)
        {
            var subject = await _context.ExamPaperSubjects
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == subjectEntryId);

            return subject?.Url;
        }

        public async Task<bool> UploadPaperAsync(IFormFile pdfFile, UploadPaperDto request)
        {
            try
            {
                if (pdfFile == null || pdfFile.Length == 0 ||
                    !pdfFile.ContentType.Equals("application/pdf"))
                    return false;

                var subject  = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId);
                var safeTitle = System.Text.RegularExpressions.Regex
                                  .Replace(request.Title.ToLower(), @"[^a-z0-9]+", "-");
                var fileName = $"papers/{safeTitle}-{Guid.NewGuid():N[..8]}.pdf";

                string r2Url;
                using (var stream = pdfFile.OpenReadStream())
                {
                    r2Url = await _r2Storage.UploadPdfAsync(stream, fileName, pdfFile.ContentType);
                }

                var examPaper = new ExamPaper
                {
                    Title      = request.Title,
                    UploadedBy = "system",
                    UploadedAt = DateTime.UtcNow,
                    ExamDate   = DateTime.UtcNow
                };

                examPaper.Departments.Add(new ExamPaperDept { DepartmentId = request.DepartmentId });
                examPaper.Posts.Add(new ExamPaperPost       { PostId       = request.PostId       });
                examPaper.Stages.Add(new ExamPaperStage     { StageId      = request.StageId      });
                examPaper.Subjects.Add(new ExamPaperSubject
                {
                    StageId     = request.StageId,
                    SubjectId   = request.SubjectId,
                    SubjectName = subject?.Name,
                    Url         = r2Url,
                    Date        = DateTime.UtcNow
                });

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