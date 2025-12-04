using Project.Domain.Entities.Base;

namespace Project.Domain.Entities;

/// <summary>
/// Represents a permission definition in the database.
/// </summary>
public class PermissionDefinition : FullAuditedEntity
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public string? ParentName { get; set; }
    public bool IsStatic { get; set; }
    public bool IsHost { get; set; }
}
