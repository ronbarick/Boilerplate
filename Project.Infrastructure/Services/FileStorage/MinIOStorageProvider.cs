using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Configuration;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// MinIO storage provider (S3-compatible)
/// </summary>
public class MinIOStorageProvider : IFileStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public string ProviderName => "MinIO";

    public MinIOStorageProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        var endpoint = _configuration["App:FileStorage:MinIO:Endpoint"] ?? "localhost:9000";
        var accessKey = _configuration["App:FileStorage:MinIO:AccessKey"];
        var secretKey = _configuration["App:FileStorage:MinIO:SecretKey"];
        var useSSL = bool.Parse(_configuration["App:FileStorage:MinIO:UseSSL"] ?? "false");
        _bucketName = _configuration["App:FileStorage:MinIO:Bucket"] ?? "files";

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("MinIO credentials are not configured.");
        }

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(useSSL)
            .Build();

        // Ensure bucket exists
        EnsureBucketExistsAsync().GetAwaiter().GetResult();
    }

    private async Task EnsureBucketExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);
        var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs);

        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs);
        }
    }

    public async Task SaveAsync(Stream stream, string storagePath, CancellationToken cancellationToken = default)
    {
        // Reset stream position if seekable
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(storagePath)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
    }

    public async Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(storagePath)
            .WithCallbackStream(async (stream) =>
            {
                await stream.CopyToAsync(memoryStream, cancellationToken);
            });

        try
        {
            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"MinIO object not found: {storagePath}", ex);
        }
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(storagePath);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }

    public async Task<string?> GeneratePublicUrlAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        // Generate pre-signed URL valid for 1 hour
        var presignedGetObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(storagePath)
            .WithExpiry(3600); // 1 hour in seconds

        var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
        return url;
    }
}
