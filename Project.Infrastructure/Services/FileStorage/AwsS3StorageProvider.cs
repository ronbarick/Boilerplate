using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Project.Domain.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// AWS S3 storage provider
/// </summary>
public class AwsS3StorageProvider : IFileStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public string ProviderName => "S3";

    public AwsS3StorageProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        var accessKey = _configuration["App:FileStorage:AWS:AccessKey"];
        var secretKey = _configuration["App:FileStorage:AWS:SecretKey"];
        var regionName = _configuration["App:FileStorage:AWS:Region"] ?? "us-east-1";
        _bucketName = _configuration["App:FileStorage:AWS:Bucket"] ?? "my-app-files";

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("AWS S3 credentials are not configured.");
        }

        var region = RegionEndpoint.GetBySystemName(regionName);
        _s3Client = new AmazonS3Client(accessKey, secretKey, region);

        // Ensure bucket exists
        EnsureBucketExistsAsync().GetAwaiter().GetResult();
    }

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            await _s3Client.PutBucketAsync(_bucketName);
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "BucketAlreadyOwnedByYou")
        {
            // Bucket already exists, ignore
        }
    }

    public async Task SaveAsync(Stream stream, string storagePath, CancellationToken cancellationToken = default)
    {
        // Reset stream position if seekable
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = storagePath,
            InputStream = stream
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
    }

    public async Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = storagePath
        };

        try
        {
            var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
            
            // Copy to memory stream to avoid disposal issues
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            
            return memoryStream;
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
        {
            throw new FileNotFoundException($"S3 object not found: {storagePath}");
        }
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = storagePath
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }

    public Task<string?> GeneratePublicUrlAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // Generate pre-signed URL valid for 1 hour
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = storagePath,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        var url = _s3Client.GetPreSignedURL(request);
        return Task.FromResult<string?>(url);
    }
}
