using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPSCReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Matches Flutter AuthService headers
    public class PapersController : ControllerBase
    {
        private readonly IPaperService _paperService;
        private readonly ApplicationDbContext _context;

        public PapersController(IPaperService paperService, ApplicationDbContext context)
        {
            _paperService = paperService;
            _context = context;
        }

        /// <summary>
        /// Get all departments for upload form dropdown
        /// </summary>
        [HttpGet("departments")]
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        {
            var departments = await _paperService.GetDepartmentsAsync();
            return Ok(departments);
        }

        /// <summary>
        /// Get posts for selected department
        /// </summary>
        [HttpGet("posts")]
        public async Task<ActionResult<List<PostDto>>> GetPosts([FromQuery] Guid departmentId)
        {
            var posts = await _paperService.GetPostsAsync(departmentId);
            return Ok(posts);
        }

        /// <summary>
        /// Get exam cycles for selected post
        /// </summary>
        [HttpGet("cycles")]
        public async Task<ActionResult<List<ExamCycleDto>>> GetExamCycles([FromQuery] Guid postId)
        {
            var cycles = await _paperService.GetExamCyclesAsync(postId);
            return Ok(cycles);
        }

        /// <summary>
        /// Get all exam stages
        /// </summary>
        [HttpGet("stages")]
        public async Task<ActionResult<List<ExamStageDto>>> GetExamStages()
        {
            var stages = await _paperService.GetExamStagesAsync();
            return Ok(stages);
        }

        /// <summary>
        /// Get all subjects
        /// </summary>
        [HttpGet("subjects")]
        public async Task<ActionResult<List<ExamSubjectDto>>> GetSubjects()
        {
            var subjects = await _paperService.GetSubjectsAsync();
            return Ok(subjects);
        }

        /// <summary>
        /// Upload paper - multipart form data endpoint
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPaper([FromForm] UploadPaperDto request, IFormFile pdfFile, [FromServices] IWebHostEnvironment env)
        {
            var webRootPath = env.WebRootPath;
            var success = await _paperService.UploadPaperAsync(pdfFile, request, webRootPath);
            return success ? Ok("Paper uploaded successfully") : BadRequest("Upload failed");
        }

        /// <summary>
        /// Get papers with optional search by department, post name, exam type - existing endpoint 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ExamPaper>>> GetPapers(
            [FromQuery] string? search = null,
            [FromQuery] Guid? stageId = null,
            [FromQuery] string? departmentName = null,
            [FromQuery] int? examYear = null,
            [FromQuery] string? stageName = null,
            [FromQuery] string? postName = null)
        {
            // Keep direct DbContext query for search performance - doesn't need service
            var query = _context.ExamPapers
                .Include(p => p.ExamStage)
                .Include(p => p.Subject)
                .Include(p => p.ExamCycle)
                    .ThenInclude(ec => ec.Post)
                    .ThenInclude(ec => ec.Department)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (stageId.HasValue)
            {
                query = query.Where(p => p.ExamStageId == stageId.Value);
            }

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                query = query.Where(p => p.ExamCycle.Department.Name.Contains(departmentName, StringComparison.OrdinalIgnoreCase));
            }

            if (examYear.HasValue)
            {
                query = query.Where(p => p.ExamCycle.ExamYear == examYear.Value);
            }

            if (!string.IsNullOrWhiteSpace(stageName))
            {
                query = query.Where(p => p.ExamStage.Name.Contains(stageName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(postName))
            {
                query = query.Where(p => p.ExamCycle.Post.Name.Contains(postName, StringComparison.OrdinalIgnoreCase));
            }

            var papers = await query.OrderByDescending(p => p.UploadedAt).ToListAsync();

            return Ok(papers);
        }
    }
}
