using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Admin.Models;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;

namespace SPSCReady.Admin.Controllers
{
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IR2StorageService _r2;

        public UploadController(ApplicationDbContext db, IR2StorageService r2)
        {
            _db = db;
            _r2 = r2;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await PopulateViewBagAsync();
            return View(new ExamPaperCreateModel());
        }

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
                    await _db.SaveChangesAsync();
                }

                string publicUrl = await _r2.UploadAsync(paper.PdfFile!, folder: "papers");

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
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/api/papers/posts")]
        public async Task<IActionResult> GetPostsByDepartment([FromQuery] int departmentId)
        {
            var posts = await _db.Posts
                .Where(p => p.DepartmentId == departmentId)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            return Json(posts);
        }

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