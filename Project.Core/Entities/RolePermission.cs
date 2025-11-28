using System;
using Project.Core.Entities.Base;

namespace Project.Core.Entities;

public class RolePermission : FullAuditedEntity
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public string PermissionName { get; set; } = null!;
    public bool IsGranted { get; set; } = true;
}
