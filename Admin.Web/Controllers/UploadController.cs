using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Models;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;

namespace MyApp.Admin.Controllers
{
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public UploadController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ── GET /Upload/Index ────────────────────────────────────────────────
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
            // ── Extra file validations (ModelState won't cover these) ────────
            for (int i = 0; i < model.SubjectPapers.Count; i++)
            {
                var p = model.SubjectPapers[i];
                if (p.PdfFile == null || p.PdfFile.Length == 0)
                {
                    ModelState.AddModelError($"SubjectPapers[{i}].PdfFile",
                        $"Paper {i + 1}: please upload a PDF.");
                    continue;
                }
                var ext = Path.GetExtension(p.PdfFile.FileName).ToLowerInvariant();
                if (ext != ".pdf")
                    ModelState.AddModelError($"SubjectPapers[{i}].PdfFile",
                        $"Paper {i + 1}: only PDF files are accepted.");

                if (p.PdfFile.Length > 20 * 1024 * 1024)
                    ModelState.AddModelError($"SubjectPapers[{i}].PdfFile",
                        $"Paper {i + 1}: file must be under 20 MB.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewBagAsync();
                return View("Index", model);
            }

            // ── 1. Create the parent ExamPaper row ───────────────────────────
            var examPaper = new ExamPaper
            {
                Title = model.Title,
                ExamDate = new DateTime(model.ExamYear, 1, 1),
                UploadedAt = DateTime.UtcNow,
                UploadedBy = User.Identity?.Name ?? "admin"
            };
            _db.ExamPapers.Add(examPaper);
            await _db.SaveChangesAsync(); // get the generated ExamPaper.Id

            int examId = examPaper.Id;

            // ── 2. ExamPaperDepartments ──────────────────────────────────────
            var examDept = new ExamPaperDept
            {
                ExamId = examId,
                DepartmentId = model.DepartmentId
            };
            _db.ExamPaperDepartments.Add(examDept);

            // ── 3. ExamPaperPosts ────────────────────────────────────────────
            var examPost = new ExamPaperPost
            {
                ExamId = examId,
                PostId = model.PostId
            };
            _db.ExamPaperPosts.Add(examPost);

            // ── 4. ExamPaperStages (unique per stage used) ───────────────────
            var uniqueStageIds = model.SubjectPapers
                .Select(p => p.StageId)
                .Distinct();

            foreach (var stageId in uniqueStageIds)
            {
                bool stageExists = await _db.ExamPaperStages
                    .AnyAsync(s => s.ExamId == examId && s.StageId == stageId);

                if (!stageExists)
                {
                    _db.ExamPaperStages.Add(new ExamPaperStage
                    {
                        ExamId = examId,
                        StageId = stageId
                    });
                }
            }

            // ── 5. ExamPaperSubjects (one per uploaded paper) ────────────────
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "papers");
            Directory.CreateDirectory(uploadsDir);

            foreach (var paper in model.SubjectPapers)
            {
                // Resolve or create the Subject row from SubjectName + StageId
                var subject = await _db.Subjects
                    .FirstOrDefaultAsync(s =>
                        s.StageId == paper.StageId &&
                        s.Name == paper.SubjectName.Trim());

                if (subject == null)
                {
                    subject = new Subject
                    {
                        Name = paper.SubjectName.Trim(),
                        StageId = paper.StageId
                    };
                    _db.Subjects.Add(subject);
                    await _db.SaveChangesAsync(); // get Subject.Id
                }

                // Save PDF to disk
                var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(paper.PdfFile!.FileName)}";
                var filePath = Path.Combine(uploadsDir, safeFileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await paper.PdfFile.CopyToAsync(stream);

                // Relative URL stored in DB  e.g. /uploads/papers/abc123_paper.pdf
                var relativeUrl = $"/uploads/papers/{safeFileName}";

                _db.ExamPaperSubjects.Add(new ExamPaperSubject
                {
                    ExamId = examId,
                    StageId = paper.StageId,
                    SubjectId = subject.Id,
                    SubjectName = paper.SubjectName.Trim(),
                    Url = relativeUrl,
                    Date = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();

            TempData["Success"] =
                $"'{model.Title}' uploaded successfully with {model.SubjectPapers.Count} paper(s).";

            return RedirectToAction(nameof(Index));
        }

        // ── API: GET /api/papers/posts?departmentId={id} ─────────────────────
        // Used by the JS dropdown to load posts when department changes.
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