using Microsoft.AspNetCore.Http;
using Project.Domain.Dtos.FileStorage;

namespace Project.Application.FileStorage;

/// <summary>
/// Application service interface for file storage operations
/// </summary>
public interface IFileStorageAppService
{
    Task<FileInfoDto> UploadAsync(IFormFile file, FileUploadInput input);
    Task<FileDownloadDto> DownloadAsync(Guid fileId);
    Task<FileInfoDto> GetFileInfoAsync(Guid fileId);
    Task DeleteAsync(Guid fileId);
    Task<string?> GetPublicUrlAsync(Guid fileId);
    Task<List<FileInfoDto>> GetFilesByEntityAsync(Guid entityId, string entityType);
}
