using System;

namespace Project.Core.Interfaces;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? UserName { get; }
    string? Email { get; }
    Guid? TenantId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string roleName);
}
