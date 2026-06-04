using Microsoft.AspNetCore.Http;

namespace SPSCReady.Application.Interfaces;

public interface IR2StorageService
{
    // Used by Admin panel (IFormFile upload)
    Task<string> UploadAsync(IFormFile file, string folder = "papers");

    // Used by PaperService / API (Stream upload)
    Task<string> UploadPdfAsync(Stream stream, string fileName, string contentType);

    Task DeleteAsync(string fileKey);
}