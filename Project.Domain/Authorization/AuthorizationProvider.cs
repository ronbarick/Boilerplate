using Project.Domain.Interfaces;

namespace Project.Domain.Authorization;

/// <summary>
/// Base class for authorization providers.
/// Inherit from this class to define permissions for your application.
/// </summary>
public abstract class AuthorizationProvider
{
    /// <summary>
    /// Override this method to define permissions.
    /// </summary>
    /// <param name="context">Permission definition context</param>
    public abstract void SetPermissions(IPermissionDefinitionContext context);
}
