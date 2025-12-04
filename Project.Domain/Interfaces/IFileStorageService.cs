using Microsoft.AspNetCore.Http;
using Project.Domain.Dtos.FileStorage;

namespace Project.Domain.Interfaces;

/// <summary>
/// Main service interface for file storage operations
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Upload a file from IFormFile
    /// </summary>
    Task<FileInfoDto> UploadAsync(IFormFile file, FileUploadInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a file from byte array
    /// </summary>
    Task<FileInfoDto> UploadBytesAsync(byte[] bytes, string fileName, string mimeType, FileUploadInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file by ID
    /// </summary>
    Task<FileDownloadDto> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get file metadata by ID
    /// </summary>
    Task<FileInfoDto> GetFileInfoAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file (soft delete)
    /// </summary>
    Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get public URL for a file (if public access is enabled)
    /// </summary>
    Task<string?> GetPublicUrlAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all files associated with a specific entity
    /// </summary>
    Task<List<FileInfoDto>> GetFilesByEntityAsync(Guid entityId, string entityType, CancellationToken cancellationToken = default);
}
