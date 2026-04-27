using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IPaperService _paperService;
        private readonly IWebHostEnvironment _env;

        public AdminController(IPaperService paperService, IWebHostEnvironment env)
        {
            _paperService = paperService;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadPaperDto request, IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return BadRequest("No PDF file uploaded.");
            }

            if (!pdfFile.ContentType.Equals("application/pdf"))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            var success = await _paperService.UploadPaperAsync(pdfFile, request, _env.WebRootPath);

            if (success)
            {
                return Ok("Paper uploaded successfully.");
            }

            return StatusCode(500, "Failed to upload paper.");
        }
    }
}
