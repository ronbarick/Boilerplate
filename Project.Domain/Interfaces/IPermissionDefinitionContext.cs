namespace Project.Domain.Interfaces;

/// <summary>
/// Context for defining permissions in the application.
/// Used by AuthorizationProvider to register permissions.
/// </summary>
public interface IPermissionDefinitionContext
{
    /// <summary>
    /// Creates a new permission definition.
    /// </summary>
    /// <param name="name">Unique name of the permission</param>
    /// <param name="displayName">Display name for the permission</param>
    /// <param name="isHostOnly">True if this permission is only for host (not for tenants)</param>
    /// <returns>The created permission definition</returns>
    Authorization.Permission CreatePermission(string name, string displayName, bool isHostOnly = false);
}
