namespace Project.Domain.Dtos.FileStorage;

/// <summary>
/// DTO containing file metadata information
/// </summary>
public class FileInfoDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public bool IsPublic { get; set; }
    public string? Folder { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? PublicUrl { get; set; }
}
