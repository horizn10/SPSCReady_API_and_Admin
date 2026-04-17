using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace SPSCReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PapersController : ControllerBase
    {
        private readonly IPaperService _paperService;

        public PapersController(IPaperService paperService)
        {
            _paperService = paperService;
        }

        [HttpGet("departments")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        {
            var departments = await _paperService.GetDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("posts")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PostDto>>> GetPosts([FromQuery] int departmentId)
        {
            var posts = await _paperService.GetPostsAsync(departmentId);
            return Ok(posts);
        }

        [HttpGet("stages")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ExamStageDto>>> GetExamStages()
        {
            var stages = await _paperService.GetExamStagesAsync();
            return Ok(stages);
        }

        [HttpGet("subjects")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ExamSubjectDto>>> GetSubjects()
        {
            var subjects = await _paperService.GetSubjectsAsync();
            return Ok(subjects);
        }

        [HttpPost("upload")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadPaper([FromForm] UploadPaperDto request, IFormFile pdfFile, [FromServices] IWebHostEnvironment env)
        {
            var webRootPath = env.WebRootPath;
            var success = await _paperService.UploadPaperAsync(pdfFile, request, webRootPath);
            return success ? Ok("Paper uploaded successfully") : BadRequest("Upload failed");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ExamPaperListDto>>> GetPapers(
            [FromQuery] string? search = null,
            [FromQuery] int? stageId = null,
            [FromQuery] string? departmentName = null,
            [FromQuery] int? examYear = null,
            [FromQuery] string? stageName = null,
            [FromQuery] string? postName = null)
        {
            var papers = await _paperService.GetPapersAsync(search, stageId, departmentName, examYear, stageName, postName);
            return Ok(papers);
        }
    }
}
