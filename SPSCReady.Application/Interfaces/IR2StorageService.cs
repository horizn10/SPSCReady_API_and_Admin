namespace SPSCReady.Application.Interfaces;

public interface IR2StorageService
{
    Task<string> UploadPdfAsync(Stream fileStream, string fileName, string contentType);
}
