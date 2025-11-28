using System;
using Project.Core.Entities.Base;

namespace Project.Core.Entities;

public class UserPermission : FullAuditedEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string PermissionName { get; set; } = null!;
    public bool IsGranted { get; set; } = true;
}
