using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Project.Domain.Dtos.FileStorage;
using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// Main file storage service implementation with validation, upload, download, and delete operations
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly IRepository<FileStorageItem> _fileRepository;
    private readonly IFileStorageProviderSelector _providerSelector;
    private readonly IFileHashService _hashService;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileStorageService> _logger;

    // Configurable settings
    private readonly long _maxFileSize;
    private readonly bool _enablePublicUrls;
    private readonly HashSet<string> _allowedExtensions;

    public FileStorageService(
        IRepository<FileStorageItem> fileRepository,
        IFileStorageProviderSelector providerSelector,
        IFileHashService hashService,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IConfiguration configuration,
        ILogger<FileStorageService> logger)
    {
        _fileRepository = fileRepository;
        _providerSelector = providerSelector;
        _hashService = hashService;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _configuration = configuration;
        _logger = logger;

        // Load configuration
        _maxFileSize = long.Parse(_configuration["App:FileStorage:MaxFileSize"] ?? "52428800"); // 50MB default
        _enablePublicUrls = bool.Parse(_configuration["App:FileStorage:EnablePublicUrls"] ?? "false");
        
        var allowedExtensionsConfig = _configuration.GetSection("App:FileStorage:AllowedExtensions").Get<string[]>();
        _allowedExtensions = allowedExtensionsConfig != null 
            ? new HashSet<string>(allowedExtensionsConfig, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".zip" }, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<FileInfoDto> UploadAsync(IFormFile file, FileUploadInput input, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null.");
        }

        // Validate file size
        if (file.Length > _maxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {_maxFileSize / 1024 / 1024}MB.");
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }

        // Validate MIME type
        var mimeType = file.ContentType;
        if (string.IsNullOrEmpty(mimeType))
        {
            throw new InvalidOperationException("MIME type is missing.");
        }

        // Open stream and upload
        using var stream = file.OpenReadStream();
        return await UploadBytesAsync(stream, file.FileName, mimeType, input, cancellationToken);
    }

    public async Task<FileInfoDto> UploadBytesAsync(byte[] bytes, string fileName, string mimeType, FileUploadInput input, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(bytes);
        return await UploadBytesAsync(stream, fileName, mimeType, input, cancellationToken);
    }

    private async Task<FileInfoDto> UploadBytesAsync(Stream stream, string fileName, string mimeType, FileUploadInput input, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        // Compute hash
        var hash = await _hashService.ComputeHashAsync(stream, cancellationToken);

        // Check for duplicates (optional - can be configured)
        var existingFile = await _fileRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Hash == hash && f.TenantId == _currentTenant.Id, cancellationToken);

        if (existingFile != null)
        {
            _logger.LogInformation("Duplicate file detected. Hash: {Hash}, Existing FileId: {FileId}", hash, existingFile.Id);
            // Return existing file info instead of uploading again
            return MapToDto(existingFile);
        }

        // Get provider
        var provider = _providerSelector.GetProvider();

        // Generate storage path: {tenantId}/{year}/{month}/{guid}{extension}
        var tenantId = _currentTenant.Id?.ToString() ?? "host";
        var now = DateTime.UtcNow;
        var fileId = Guid.NewGuid();
        var storagePath = $"{tenantId}/{now.Year}/{now.Month:D2}/{fileId}{extension}";

        // Add folder if specified
        if (!string.IsNullOrEmpty(input.Folder))
        {
            storagePath = $"{tenantId}/{input.Folder}/{now.Year}/{now.Month:D2}/{fileId}{extension}";
        }

        // Save to storage provider
        await provider.SaveAsync(stream, storagePath, cancellationToken);

        // Create metadata entity
        var fileEntity = new FileStorageItem
        {
            Id = fileId,
            FileName = fileName,
            Extension = extension,
            MimeType = mimeType,
            Size = stream.Length,
            Hash = hash,
            StorageProvider = provider.ProviderName,
            StoragePath = storagePath,
            EntityId = input.EntityId,
            EntityType = input.EntityType,
            IsPublic = input.IsPublic,
            Folder = input.Folder,
            TenantId = _currentTenant.Id,
            CreatedBy = _currentUser.Id,
            CreatedOn = DateTime.UtcNow
        };

        // Save to database
        await _fileRepository.InsertAsync(fileEntity, cancellationToken: cancellationToken);

        _logger.LogInformation("File uploaded successfully. FileId: {FileId}, Provider: {Provider}, Size: {Size} bytes", 
            fileId, provider.ProviderName, stream.Length);

        return MapToDto(fileEntity);
    }

    public async Task<FileDownloadDto> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        // Get file metadata
        var fileEntity = await _fileRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted, cancellationToken);

        if (fileEntity == null)
        {
            throw new FileNotFoundException($"File with ID {fileId} not found.");
        }

        // Check tenant boundary
        if (fileEntity.TenantId != _currentTenant.Id)
        {
            throw new UnauthorizedAccessException("Access denied to file from different tenant.");
        }

        // Get provider and read file
        var provider = _providerSelector.GetProvider(fileEntity.StorageProvider);
        var stream = await provider.ReadAsync(fileEntity.StoragePath, cancellationToken);

        _logger.LogInformation("File downloaded. FileId: {FileId}, Provider: {Provider}", fileId, provider.ProviderName);

        return new FileDownloadDto
        {
            Stream = stream,
            FileName = fileEntity.FileName,
            MimeType = fileEntity.MimeType,
            Size = fileEntity.Size
        };
    }

    public async Task<FileInfoDto> GetFileInfoAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var fileEntity = await _fileRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted, cancellationToken);

        if (fileEntity == null)
        {
            throw new FileNotFoundException($"File with ID {fileId} not found.");
        }

        // Check tenant boundary
        if (fileEntity.TenantId != _currentTenant.Id)
        {
            throw new UnauthorizedAccessException("Access denied to file from different tenant.");
        }

        return MapToDto(fileEntity);
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var fileEntity = await _fileRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted, cancellationToken);

        if (fileEntity == null)
        {
            throw new FileNotFoundException($"File with ID {fileId} not found.");
        }

        // Check tenant boundary
        if (fileEntity.TenantId != _currentTenant.Id)
        {
            throw new UnauthorizedAccessException("Access denied to file from different tenant.");
        }

        // Soft delete in database
        await _fileRepository.DeleteAsync(fileEntity, cancellationToken: cancellationToken);

        // Optionally delete from storage provider (can be done in background job)
        try
        {
            var provider = _providerSelector.GetProvider(fileEntity.StorageProvider);
            await provider.DeleteAsync(fileEntity.StoragePath, cancellationToken);
            _logger.LogInformation("File deleted from storage. FileId: {FileId}, Provider: {Provider}", fileId, provider.ProviderName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file from storage provider. FileId: {FileId}", fileId);
            // Don't throw - soft delete in DB is sufficient
        }
    }

    public async Task<string?> GetPublicUrlAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        if (!_enablePublicUrls)
        {
            throw new InvalidOperationException("Public URLs are disabled in configuration.");
        }

        var fileEntity = await _fileRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted, cancellationToken);

        if (fileEntity == null)
        {
            throw new FileNotFoundException($"File with ID {fileId} not found.");
        }

        if (!fileEntity.IsPublic)
        {
            throw new InvalidOperationException("File is not marked as public.");
        }

        // Check tenant boundary
        if (fileEntity.TenantId != _currentTenant.Id)
        {
            throw new UnauthorizedAccessException("Access denied to file from different tenant.");
        }

        var provider = _providerSelector.GetProvider(fileEntity.StorageProvider);
        return await provider.GeneratePublicUrlAsync(fileEntity.StoragePath, cancellationToken);
    }

    public async Task<List<FileInfoDto>> GetFilesByEntityAsync(Guid entityId, string entityType, CancellationToken cancellationToken = default)
    {
        var files = await _fileRepository.GetQueryable()
            .Where(f => f.EntityId == entityId && f.EntityType == entityType && !f.IsDeleted && f.TenantId == _currentTenant.Id)
            .OrderByDescending(f => f.CreatedOn)
            .ToListAsync(cancellationToken);

        return files.Select(MapToDto).ToList();
    }

    private FileInfoDto MapToDto(FileStorageItem entity)
    {
        return new FileInfoDto
        {
            Id = entity.Id,
            FileName = entity.FileName,
            Extension = entity.Extension,
            MimeType = entity.MimeType,
            Size = entity.Size,
            StorageProvider = entity.StorageProvider,
            EntityId = entity.EntityId,
            EntityType = entity.EntityType,
            IsPublic = entity.IsPublic,
            Folder = entity.Folder,
            CreatedOn = entity.CreatedOn,
            CreatedBy = entity.CreatedBy,
            PublicUrl = null // Will be populated if requested via GetPublicUrlAsync
        };
    }
}
