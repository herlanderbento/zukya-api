using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Movu.Infra.Storage;
using Zukya.Application.Common.Interfaces;

namespace Zukya.Infra.Services.Storage;

public class StorageService(IAmazonS3 s3Client, IOptions<StorageServiceOptions> options)
    : IStorageService
{
    private readonly StorageServiceOptions _options = options.Value;

    public async Task Delete(string filePath, CancellationToken cancellationToken)
    {
        // Extract relative path from URL if it's a full URL
        var key = ExtractRelativePath(filePath);

        var deleteRequest = new DeleteObjectRequest { BucketName = _options.BucketName, Key = key };

        await s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }

    private string ExtractRelativePath(string pathOrUrl)
    {
        // If it's a full URL, extract the relative path
        if (!pathOrUrl.StartsWith("http://") && !pathOrUrl.StartsWith("https://")) return pathOrUrl;
        var uri = new Uri(pathOrUrl);
        return uri.AbsolutePath.TrimStart('/');

        // Otherwise, return as-is (already a relative path)
    }

    public string GetPublicUrl(string filePath)
    {
        // If PublicUrl is configured, use it to build the full URL
        if (string.IsNullOrWhiteSpace(_options.PublicUrl)) return filePath;
        var baseUrl = _options.PublicUrl.TrimEnd('/');
        return $"{baseUrl}/{filePath}";

        // Otherwise, return the file path as-is (for backward compatibility)
    }

    public async Task<string> Upload(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken
    )
    {
        // Cloudflare R2 doesn't support chunked encoding with payload trailer
        // Read stream into memory when using R2 (indicated by EndpointUrl being set)
        Stream streamToUse = fileStream;
        MemoryStream? memoryStream = null;

        if (!string.IsNullOrWhiteSpace(_options.EndpointUrl))
        {
            memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            streamToUse = memoryStream;
        }

        try
        {
            // Ensure ContentType is set, default to image/jpeg if empty
            var finalContentType = string.IsNullOrWhiteSpace(contentType)
                ? GetContentTypeFromFileName(fileName)
                : contentType;

            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileName,
                InputStream = streamToUse,
                ContentType = finalContentType
            };

            // Cloudflare R2 specific configurations
            // Disable payload signing and checksum validation to avoid issues with R2
            if (!string.IsNullOrWhiteSpace(_options.EndpointUrl))
            {
                putRequest.DisablePayloadSigning = true;
                putRequest.DisableDefaultChecksumValidation = true;

                // Don't set CannedACL - Cloudflare R2 handles public access at bucket level
                // Setting CannedACL may cause issues with R2

                // Add cache control headers for better image serving
                putRequest.Headers.CacheControl = "public, max-age=31536000"; // 1 year cache

                // Ensure Content-Type header is explicitly set
                putRequest.Headers.ContentType = finalContentType;
            }

            await s3Client.PutObjectAsync(putRequest, cancellationToken);
            return fileName;
        }
        finally
        {
            // Dispose the memory stream after upload is complete
            memoryStream?.DisposeAsync();
        }
    }

    private static string GetContentTypeFromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
