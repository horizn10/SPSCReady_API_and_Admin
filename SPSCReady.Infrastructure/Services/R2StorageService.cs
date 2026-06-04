using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SPSCReady.Application.Interfaces; // Assuming this matches your project structure

namespace SPSCReady.Infrastructure.Services;

public class R2StorageService : IR2StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucket;
    private readonly string _publicBaseUrl;

    public R2StorageService(IConfiguration config)
    {
        var r2 = config.GetSection("Cloudflare:R2");

        // 1. Defensive Checks: Prevent silent fallbacks to local machine AWS credentials
        var accessKey = r2["AccessKeyId"];
        var secretKey = r2["SecretAccessKey"];
        var accountId = r2["AccountId"];
        var bucketName = r2["BucketName"];
        var publicBaseUrl = r2["PublicBaseUrl"];

        if (string.IsNullOrWhiteSpace(accessKey) ||
            string.IsNullOrWhiteSpace(secretKey) ||
            string.IsNullOrWhiteSpace(accountId) ||
            string.IsNullOrWhiteSpace(bucketName))
        {
            throw new ArgumentException("Cloudflare R2 configuration values are missing or empty in appsettings.json.");
        }

        var credentials = new BasicAWSCredentials(accessKey, secretKey);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true,
            // 2. The Critical Fix: Use "auto" instead of a hardcoded AWS region
            AuthenticationRegion = "auto"
        };

        _s3Client = new AmazonS3Client(credentials, s3Config);
        _bucket = bucketName;
        _publicBaseUrl = publicBaseUrl?.TrimEnd('/') ?? string.Empty;
    }

    // ── IFormFile version — used by Admin panel ──────────────────────────
    public async Task<string> UploadAsync(IFormFile file, string folder = "papers")
    {
        var year = DateTime.UtcNow.Year.ToString();
        var safeFileName = $"{Guid.NewGuid()}_{SanitizeFileName(file.FileName)}";
        var key = $"{folder}/{year}/{safeFileName}";

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = ms,
            ContentType = file.ContentType,
            UseChunkEncoding = false,
            DisablePayloadSigning = true
        };
        request.Headers.ContentLength = ms.Length;

        await _s3Client.PutObjectAsync(request);

        return $"{_publicBaseUrl}/{key}";
    }

    // ── Stream version — used by PaperService / API ──────────────────────
    public async Task<string> UploadPdfAsync(Stream stream, string fileName, string contentType)
    {
        // Read into MemoryStream so content-length is known upfront
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = fileName,
            InputStream = ms,
            ContentType = contentType,
            UseChunkEncoding = false,
            DisablePayloadSigning = true
        };
        request.Headers.ContentLength = ms.Length;

        await _s3Client.PutObjectAsync(request);

        return $"{_publicBaseUrl}/{fileName}";
    }

    // ── Delete ───────────────────────────────────────────────────────────
    public async Task DeleteAsync(string fileKey)
    {
        if (fileKey.StartsWith("http"))
            fileKey = new Uri(fileKey).AbsolutePath.TrimStart('/');

        await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = fileKey
        });
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var safe = string.Concat(name
            .Replace(" ", "_")
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'));
        return $"{safe}{ext}";
    }
}