using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.Infrastructure.Services;

public class R2StorageService : IR2StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucket;
    private readonly string _publicBaseUrl;

    public R2StorageService(IConfiguration config)
    {
        var r2 = config.GetSection("Cloudflare:R2");

        var credentials = new BasicAWSCredentials(
            r2["AccessKeyId"],
            r2["SecretAccessKey"]);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{r2["AccountId"]}.r2.cloudflarestorage.com",
            ForcePathStyle = true,
            RegionEndpoint = RegionEndpoint.USEast1
        };

        _s3Client = new AmazonS3Client(credentials, s3Config);
        _bucket = r2["BucketName"]!;
        _publicBaseUrl = r2["PublicBaseUrl"]!.TrimEnd('/');
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
