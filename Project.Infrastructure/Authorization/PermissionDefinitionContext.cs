using Project.Core.Authorization;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Authorization;

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
