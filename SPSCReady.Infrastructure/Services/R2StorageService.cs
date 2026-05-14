using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using SPSCReady.Application.Interfaces;

namespace SPSCReady.Infrastructure.Services;

public class R2StorageService : IR2StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _publicBaseUrl;

    public R2StorageService(IConfiguration config)
    {
        _bucketName    = config["Cloudflare:R2:BucketName"]!;
        _publicBaseUrl = config["Cloudflare:R2:PublicBaseUrl"]!;
        var accountId  = config["Cloudflare:R2:AccountId"]!;

        _s3Client = new AmazonS3Client(
            config["Cloudflare:R2:AccessKeyId"],
            config["Cloudflare:R2:SecretAccessKey"],
            new AmazonS3Config
            {
                ServiceURL    = $"https://{accountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true
            }
        );
    }

    public async Task<string> UploadPdfAsync(Stream fileStream, string fileName, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName  = _bucketName,
            Key         = fileName,
            InputStream = fileStream,
            ContentType = contentType
        };
        await _s3Client.PutObjectAsync(request);
        return $"{_publicBaseUrl}/{fileName}";
    }
}
