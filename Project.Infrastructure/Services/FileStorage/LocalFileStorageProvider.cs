using Microsoft.Extensions.Configuration;
using Project.Domain.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// Local file system storage provider
/// </summary>
public class LocalFileStorageProvider : IFileStorageProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _rootPath;

    public string ProviderName => "Local";

    public LocalFileStorageProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _rootPath = _configuration["App:FileStorage:Local:RootPath"] ?? "D:\\FileStorage";

        // Ensure root directory exists
        if (!Directory.Exists(_rootPath))
        {
            Directory.CreateDirectory(_rootPath);
        }
    }

    public async Task SaveAsync(Stream stream, string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storagePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    public Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storagePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found at path: {storagePath}");
        }

        Stream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return Task.FromResult(fileStream);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storagePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<string?> GeneratePublicUrlAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var publicUrlBase = _configuration["App:FileStorage:Local:PublicUrlBase"];

        if (string.IsNullOrEmpty(publicUrlBase))
        {
            return Task.FromResult<string?>(null);
        }

        // Generate URL: https://localhost:5001/files/{storagePath}
        var publicUrl = $"{publicUrlBase.TrimEnd('/')}/{storagePath.Replace("\\", "/")}";
        return Task.FromResult<string?>(publicUrl);
    }
}
