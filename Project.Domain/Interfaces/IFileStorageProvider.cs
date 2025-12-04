namespace Project.Domain.Interfaces;

/// <summary>
/// Strategy pattern interface for different storage providers (Local, Azure, S3, MinIO)
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    /// Name of the provider (Local, Azure, S3, MinIO)
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Save a file to storage
    /// </summary>
    Task SaveAsync(Stream stream, string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a file from storage
    /// </summary>
    Task<Stream> ReadAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from storage
    /// </summary>
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a public URL for the file (if supported)
    /// </summary>
    Task<string?> GeneratePublicUrlAsync(string storagePath, CancellationToken cancellationToken = default);
}
