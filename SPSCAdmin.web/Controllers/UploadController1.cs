using Microsoft.AspNetCore.Mvc;
using SPSCAdmin.Web.Models;
using System.Net.Http.Headers;

namespace SPSCAdmin.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public UploadController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UploadPaperViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(UploadPaperViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                // 1. Create the HTTP Client
                var client = _httpClientFactory.CreateClient();

                // IMPORTANT: Get the API URL from appsettings.json
                var apiUrl = _config["ApiSettings:BaseUrl"] + "/api/admin/upload";

                // 2. Package the data for a file upload
                using var formContent = new MultipartFormDataContent();

                formContent.Add(new StringContent(model.Title), "Title");
                formContent.Add(new StringContent(model.ExamCycleId.ToString()), "ExamCycleId");
                formContent.Add(new StringContent(model.ExamStageId.ToString()), "ExamStageId");
                formContent.Add(new StringContent(model.SubjectId.ToString()), "SubjectId");

                // Attach the PDF
                var streamContent = new StreamContent(model.PdfFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.PdfFile.ContentType);
                formContent.Add(streamContent, "PdfFile", model.PdfFile.FileName);

                // 3. Send the request to the API
                var response = await client.PostAsync(apiUrl, formContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Paper securely uploaded to the API!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", $"API Error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Connection Error: {ex.Message}. Is the API running?");
            }

            return View(model);
        }
    }
}