using System;
using System.Collections.Generic;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities;

public class Role : FullAuditedEntity
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsStatic { get; set; } = false;
    public bool IsDefault { get; set; } = false;
    
    // Navigation properties
    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}
