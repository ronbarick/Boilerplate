using Project.Core.Entities.Base;

namespace Project.Core.Entities;

/// <summary>
/// Entity to store file metadata. Raw file bytes are stored in blob storage.
/// </summary>
public class FileStorageItem : FullAuditedEntity
{
    /// <summary>
    /// Original filename (e.g., "document.pdf")
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File extension (e.g., ".pdf", ".jpg")
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// MIME type (e.g., "application/pdf", "image/jpeg")
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// SHA256 hash for integrity verification and duplicate detection
    /// </summary>
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// Storage provider used (Local, Azure, S3, MinIO)
    /// </summary>
    public string StorageProvider { get; set; } = string.Empty;

    /// <summary>
    /// Path or blob key where file is physically stored
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// Optional reference to related entity (e.g., StudentId, ClaimId)
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Fully qualified type name of related entity (e.g., "Project.Core.Entities.Student")
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Whether this file can be accessed publicly without authentication
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Optional folder/category for organization (e.g., "reports", "documents", "images")
    /// </summary>
    public string? Folder { get; set; }
}
