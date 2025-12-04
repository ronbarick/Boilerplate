namespace Project.Domain.Dtos.FileStorage;

/// <summary>
/// Input DTO for file upload operations
/// </summary>
public class FileUploadInput
{
    /// <summary>
    /// Optional reference to related entity (e.g., StudentId, ClaimId)
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Fully qualified type name of related entity (e.g., "Project.Domain.Entities.Student")
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Whether this file should be publicly accessible without authentication
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Optional folder/category for organization (e.g., "reports", "documents", "images")
    /// </summary>
    public string? Folder { get; set; }
}
