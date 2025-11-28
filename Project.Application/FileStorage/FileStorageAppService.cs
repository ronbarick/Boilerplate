using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Project.Application.Services;
using Project.Core.Attributes;
using Project.Core.Constants;
using Project.Core.Dtos.FileStorage;
using Project.Core.Interfaces;
using Project.Core.Localization;

namespace Project.Application.FileStorage;

/// <summary>
/// Application service for file storage operations with permission checks and auditing
/// </summary>
public class FileStorageAppService : AppServiceBase, IFileStorageAppService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<FileStorageAppService> _logger;

    public FileStorageAppService(
        IFileStorageService fileStorageService,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager,
        ILogger<FileStorageAppService> logger)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a file
    /// </summary>
    [RequiresPermission(PermissionNames.Pages_FileStorage, PermissionNames.Pages_FileStorage_Upload)]
    public async Task<FileInfoDto> UploadAsync(IFormFile file, FileUploadInput input)
    {
        _logger.LogInformation("User {UserId} uploading file: {FileName}, Size: {Size} bytes", 
            CurrentUser.Id, file.FileName, file.Length);

        var result = await _fileStorageService.UploadAsync(file, input);

        _logger.LogInformation("File uploaded successfully. FileId: {FileId}, Provider: {Provider}", 
            result.Id, result.StorageProvider);

        return result;
    }

    /// <summary>
    /// Download a file
    /// </summary>
    [RequiresPermission(PermissionNames.Pages_FileStorage, PermissionNames.Pages_FileStorage_Download)]
    public async Task<FileDownloadDto> DownloadAsync(Guid fileId)
    {
        _logger.LogInformation("User {UserId} downloading file: {FileId}", CurrentUser.Id, fileId);

        var result = await _fileStorageService.DownloadAsync(fileId);

        _logger.LogInformation("File downloaded successfully. FileId: {FileId}, FileName: {FileName}", 
            fileId, result.FileName);

        return result;
    }

    /// <summary>
    /// Get file metadata
    /// </summary>
    [RequiresPermission(PermissionNames.Pages_FileStorage)]
    public async Task<FileInfoDto> GetFileInfoAsync(Guid fileId)
    {
        return await _fileStorageService.GetFileInfoAsync(fileId);
    }

    /// <summary>
    /// Delete a file (soft delete)
    /// </summary>
    [RequiresPermission(PermissionNames.Pages_FileStorage, PermissionNames.Pages_FileStorage_Delete)]
    public async Task DeleteAsync(Guid fileId)
    {
        _logger.LogInformation("User {UserId} deleting file: {FileId}", CurrentUser.Id, fileId);

        await _fileStorageService.DeleteAsync(fileId);

        _logger.LogInformation("File deleted successfully. FileId: {FileId}", fileId);
    }

    /// <summary>
    /// Get public URL for a file (if enabled and file is public)
    /// </summary>
    [RequiresPermission(PermissionNames.Pages_FileStorage)]
    public async Task<string?> GetPublicUrlAsync(Guid fileId)
    {
        return await _fileStorageService.GetPublicUrlAsync(fileId);
    }

    /// <summary>
    /// Get all files associated with a specific entity
    /// </summary>
    [RequiresPermission(PermissionNames.Pages_FileStorage)]
    public async Task<List<FileInfoDto>> GetFilesByEntityAsync(Guid entityId, string entityType)
    {
        return await _fileStorageService.GetFilesByEntityAsync(entityId, entityType);
    }
}
