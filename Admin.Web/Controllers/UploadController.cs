using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Admin.Models;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace SPSCReady.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IR2StorageService _r2;

        public UploadController(ApplicationDbContext db, IR2StorageService r2)
        {
            _db = db;
            _r2 = r2;
        }

        // ── GET /Upload ──────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await PopulateViewBagAsync();
            return View(new ExamPaperCreateModel());
        }

        // ── POST /Upload/Create ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamPaperCreateModel model)
        {
            for (int i = 0; i < model.SubjectPapers.Count; i++)
            {
                var p = model.SubjectPapers[i];
                if (p.PdfFile == null || p.PdfFile.Length == 0)
                {
                    ModelState.AddModelError($"SubjectPapers[{i}].PdfFile", $"Paper {i + 1}: please upload a PDF.");
                    continue;
                }
                var ext = Path.GetExtension(p.PdfFile.FileName).ToLowerInvariant();
                if (ext != ".pdf")
                    ModelState.AddModelError($"SubjectPapers[{i}].PdfFile", $"Paper {i + 1}: only PDF files are accepted.");
                if (p.PdfFile.Length > 20 * 1024 * 1024)
                    ModelState.AddModelError($"SubjectPapers[{i}].PdfFile", $"Paper {i + 1}: file must be under 20 MB.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewBagAsync();
                return View("Index", model);
            }

            var examPaper = new ExamPaper
            {
                Title = model.Title,
                ExamDate = new DateTime(model.ExamYear, 1, 1),
                UploadedAt = DateTime.UtcNow,
                UploadedBy = User.Identity?.Name ?? "admin"
            };
            _db.ExamPapers.Add(examPaper);
            await _db.SaveChangesAsync();
            int examId = examPaper.Id;

            _db.ExamPaperDepartments.Add(new ExamPaperDept { ExamId = examId, DepartmentId = model.DepartmentId });
            _db.ExamPaperPosts.Add(new ExamPaperPost { ExamId = examId, PostId = model.PostId });

            foreach (var stageId in model.SubjectPapers.Select(p => p.StageId).Distinct())
            {
                bool exists = await _db.ExamPaperStages
                    .AnyAsync(s => s.ExamId == examId && s.StageId == stageId);
                if (!exists)
                    _db.ExamPaperStages.Add(new ExamPaperStage { ExamId = examId, StageId = stageId });
            }

            foreach (var paper in model.SubjectPapers)
            {
                var subject = await _db.Subjects
                    .FirstOrDefaultAsync(s => s.StageId == paper.StageId && s.Name == paper.SubjectName.Trim());

                if (subject == null)
                {
                    subject = new Subject { Name = paper.SubjectName.Trim(), StageId = paper.StageId };
                    _db.Subjects.Add(subject);
                    await _db.SaveChangesAsync();
                }

                using var ms = new MemoryStream();
                await paper.PdfFile!.CopyToAsync(ms);
                ms.Position = 0;
                var safeFileName = $"papers/{DateTime.UtcNow.Year}/{Guid.NewGuid()}_{Path.GetFileName(paper.PdfFile.FileName)}";
                string publicUrl = await _r2.UploadPdfAsync(ms, safeFileName, paper.PdfFile.ContentType);

                _db.ExamPaperSubjects.Add(new ExamPaperSubject
                {
                    ExamId = examId,
                    StageId = paper.StageId,
                    SubjectId = subject.Id,
                    SubjectName = paper.SubjectName.Trim(),
                    Url = publicUrl,
                    Date = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"'{model.Title}' uploaded with {model.SubjectPapers.Count} paper(s).";
            TempData["ActiveTab"] = "upload";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Upload/Papers  (AJAX — filter papers for Remove tab) ────────
        [HttpGet]
        public async Task<IActionResult> Papers(int? departmentId, int? postId)
        {
            var query = _db.ExamPaperSubjects
                .Include(s => s.ExamPaper)
                    .ThenInclude(e => e.Departments).ThenInclude(d => d.Department)
                .Include(s => s.ExamPaper)
                    .ThenInclude(e => e.Posts).ThenInclude(p => p.Post)
                .Include(s => s.Stage)
                .AsNoTracking();

            if (departmentId.HasValue)
                query = query.Where(s => s.ExamPaper.Departments
                    .Any(d => d.DepartmentId == departmentId.Value));

            if (postId.HasValue)
                query = query.Where(s => s.ExamPaper.Posts
                    .Any(p => p.PostId == postId.Value));

            var papers = await query
                .OrderByDescending(s => s.ExamPaper.UploadedAt)
                .Select(s => new
                {
                    id = s.Id,
                    title = s.ExamPaper.Title,
                    subjectName = s.SubjectName ?? s.ExamPaper.Title,
                    stageName = s.Stage.Name,
                    year = s.ExamPaper.ExamDate != null
                                    ? s.ExamPaper.ExamDate.Value.Year
                                    : s.ExamPaper.UploadedAt.Year,
                    url = s.Url,
                    department = s.ExamPaper.Departments
                                    .Select(d => d.Department.Name).FirstOrDefault(),
                    post = s.ExamPaper.Posts
                                    .Select(p => p.Post.Name).FirstOrDefault()
                })
                .ToListAsync();

            return Json(papers);
        }

        // ── POST /Upload/DeletePaper ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaper(int id)
        {
            var subject = await _db.ExamPaperSubjects
                .Include(s => s.ExamPaper)
                    .ThenInclude(e => e.Subjects)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return Json(new { success = false, message = "Paper not found." });

            // Delete from R2 if URL exists
            if (!string.IsNullOrEmpty(subject.Url))
            {
                try { await _r2.DeleteAsync(subject.Url); }
                catch { /* log but don't block */ }
            }

            var examPaper = subject.ExamPaper;

            // Remove this subject entry
            _db.ExamPaperSubjects.Remove(subject);
            await _db.SaveChangesAsync();

            // If no more subjects remain, delete the parent ExamPaper and its relations
            if (!examPaper.Subjects.Any(s => s.Id != id))
            {
                _db.ExamPaperDepartments.RemoveRange(
                    _db.ExamPaperDepartments.Where(d => d.ExamId == examPaper.Id));
                _db.ExamPaperPosts.RemoveRange(
                    _db.ExamPaperPosts.Where(p => p.ExamId == examPaper.Id));
                _db.ExamPaperStages.RemoveRange(
                    _db.ExamPaperStages.Where(s => s.ExamId == examPaper.Id));
                _db.ExamPapers.Remove(examPaper);
                await _db.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Paper deleted successfully." });
        }

        // ── API: GET /api/papers/posts?departmentId={id} ─────────────────────
        [HttpGet("/api/papers/posts")]
        public async Task<IActionResult> GetPostsByDepartment([FromQuery] int departmentId)
        {
            var posts = await _db.Posts
                .Where(p => p.DepartmentId == departmentId)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            return Json(posts);
        }

        // ── Helper ───────────────────────────────────────────────────────────
        private async Task PopulateViewBagAsync()
        {
            ViewBag.Departments = await _db.Departments
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .ToListAsync();

            ViewBag.Stages = await _db.ExamStages
                .OrderBy(s => s.Name)
                .Select(s => new ExamStageDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }
    }
}