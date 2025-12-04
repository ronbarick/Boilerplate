using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Project.Domain.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// Azure Blob Storage provider
/// </summary>
public class AzureBlobStorageProvider : IFileStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public string ProviderName => "Azure";

    public AzureBlobStorageProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = _configuration["App:FileStorage:Azure:ConnectionString"];
        _containerName = _configuration["App:FileStorage:Azure:Container"] ?? "files";

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        
        // Ensure container exists
        EnsureContainerExistsAsync().GetAwaiter().GetResult();
    }

    private async Task EnsureContainerExistsAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
    }

    public async Task SaveAsync(Stream stream, string storagePath, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(storagePath);

        // Reset stream position if seekable
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
    }

    public async Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(storagePath);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            throw new FileNotFoundException($"Blob not found: {storagePath}");
        }

        var response = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        return response;
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(storagePath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string?> GeneratePublicUrlAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(storagePath);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return null;
        }

        // Generate SAS token valid for 1 hour
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = storagePath,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
