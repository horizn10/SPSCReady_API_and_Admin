using Microsoft.AspNetCore.Mvc;
using SPSCReady.Application.DTOs;
using SPSCReady.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace SPSCReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IPaperService _paperService;
        private readonly IConfiguration _configuration;
        private readonly IR2StorageService _r2;

        public AdminController(IPaperService paperService, IConfiguration configuration, IR2StorageService r2)
        {
            _paperService = paperService;
            _configuration = configuration;
            _r2 = r2;
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

        [HttpGet("test-r2-debug")]
        public IActionResult TestR2Debug()
        {
            try
            {
                var accessKey = _configuration["Cloudflare:R2:AccessKeyId"];
                var secretKey = _configuration["Cloudflare:R2:SecretAccessKey"];
                var accountId = _configuration["Cloudflare:R2:AccountId"];
                var bucket = _configuration["Cloudflare:R2:BucketName"];
                var publicUrl = _configuration["Cloudflare:R2:PublicBaseUrl"];

                return Ok(new
                {
                    success = true,
                    message = "Configuration loaded successfully",
                    accessKey = string.IsNullOrEmpty(accessKey) ? "NOT SET" : (accessKey.Substring(0, Math.Min(10, accessKey.Length)) + "****"),
                    secretKey = string.IsNullOrEmpty(secretKey) ? "NOT SET" : (secretKey.Substring(0, Math.Min(10, secretKey.Length)) + "****"),
                    accountId = accountId ?? "NOT SET",
                    bucket = bucket ?? "NOT SET",
                    publicUrl = publicUrl ?? "NOT SET",
                    allLoaded = !string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey) && !string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(bucket)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("test-r2-upload")]
        public async Task<IActionResult> TestR2Upload()
        {
            try
            {
                var testContent = System.Text.Encoding.UTF8.GetBytes("Test file from SPSCReady API - " + DateTime.UtcNow);
                var memoryStream = new MemoryStream(testContent);
                var file = new FormFile(memoryStream, 0, memoryStream.Length, "test", $"test-{DateTime.UtcNow:yyyyMMddHHmmss}.txt")
                {
                    Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
                    ContentType = "text/plain"
                };

                var url = await _r2.UploadAsync(file, folder: "test");
                return Ok(new { success = true, message = "File uploaded successfully", url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }
    }
}
