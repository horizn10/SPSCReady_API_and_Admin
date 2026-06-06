using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Admin.Models;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace SPSCReady.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    
    
        public class HomeController : Controller
        {
            private readonly ApplicationDbContext _db;

            public HomeController(ApplicationDbContext db)
            {
                _db = db;
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                // ── Stats ────────────────────────────────────────────────────────
                int totalUsers = await _db.Users.CountAsync();
                int totalPapers = await _db.ExamPaperSubjects.CountAsync();

                // ── Papers list ──────────────────────────────────────────────────
                // Joins ExamPapers with ExamPaperSubjects to get per-paper info.
                // TotalViews is a placeholder (0) until you add a views/analytics table.
                var papers = await _db.ExamPapers
                    .OrderByDescending(e => e.UploadedAt)
                    .Take(20)
                    .Select(e => new PaperPerformanceDto
                    {
                        Id = e.Id,
                        ExamTitle = e.Title,
                        Year = e.ExamDate != null ? e.ExamDate.Value.Year : e.UploadedAt.Year,
                        Status = "Published",
                        TotalViews = 0   // replace with real view count when available
                    })
                    .ToListAsync();

                var model = new OverviewViewModel
                {
                    TotalActiveUsers = totalUsers,
                    TotalPapers = totalPapers,
                    TotalViews = 0,  // replace with real analytics sum
                    Papers = papers
                };



                return View(model);
            }
        }
    }
