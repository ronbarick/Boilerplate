using System;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities;

public class UserRole : FullAuditedEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
