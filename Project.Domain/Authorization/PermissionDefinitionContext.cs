using Project.Domain.Authorization;
using Project.Domain.Interfaces;

namespace Project.Domain.Authorization;

/// <summary>
/// Implementation of IPermissionDefinitionContext for collecting permission definitions.
/// </summary>
public class PermissionDefinitionContext : IPermissionDefinitionContext
{
    public List<Permission> Permissions { get; } = new();
    
    public Permission CreatePermission(string name, string displayName, bool isHostOnly = false)
    {
        var permission = new Permission(name, displayName, isHostOnly);
        Permissions.Add(permission);
        return permission;
    }
}
