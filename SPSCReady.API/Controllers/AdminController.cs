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

        public AdminController(IPaperService paperService)
        {
            _paperService = paperService;
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

            var success = await _paperService.UploadPaperAsync(pdfFile, request);

            if (success)
            {
                return Ok("Paper uploaded successfully.");
            }

            return StatusCode(500, "Failed to upload paper.");
        }
    }
}
