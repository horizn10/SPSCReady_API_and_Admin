using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using SPSCReady.Domain.Entities;
using SPSCReady.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
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
        public async Task<IActionResult> UploadPaper([FromForm] UploadPaperDto request, IFormFile pdfFile)
        {
            var success = await _paperService.UploadPaperAsync(pdfFile, request);
            return success ? Ok("Paper uploaded successfully") : BadRequest("Upload failed");
        }

[HttpPost("multi-upload")]
        [AllowAnonymous]
        public async Task<IActionResult> MultiUploadPaper(string Title, int DepartmentId, int PostId, int ExamYear, string SubjectPapersJson, List<IFormFile> PdfFiles, [FromServices] IWebHostEnvironment env)
        {
            Console.WriteLine($"[API HIT] Title={Title}, Dept={DepartmentId}, Post={PostId}, Year={ExamYear}, Files={PdfFiles?.Count ?? 0}, Json={SubjectPapersJson}");
            try
            {
                var subjectPapers = JsonSerializer.Deserialize<List<SubjectPaperDto>>(SubjectPapersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (subjectPapers == null || PdfFiles == null || subjectPapers.Count != PdfFiles.Count)
                    return BadRequest("Invalid data: Subject papers count must match PDF files count");

                var requestDto = new MultiUploadPaperDto
                {
                    Title = Title,
                    DepartmentId = DepartmentId,
                    PostId = PostId,
                    ExamYear = ExamYear,
                    SubjectPapers = subjectPapers
                };

                var webRootPath = env.WebRootPath;
                var success = await _paperService.MultiUploadPaperAsync(PdfFiles, requestDto, webRootPath);
                return success ? Ok("Papers uploaded successfully") : BadRequest("Multi-upload failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API ERROR] {ex}");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
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

        [HttpGet("{subjectEntryId}/pdf-url")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPdfUrl(int subjectEntryId)
        {
            var url = await _paperService.GetPdfUrlAsync(subjectEntryId);

            if (url == null || url.Trim().Length == 0)
                return NotFound(new { message = "PDF URL not found for this paper." });

            return Ok(new { url = url });
        }
    }
}
