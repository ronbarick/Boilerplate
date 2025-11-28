using Microsoft.AspNetCore.Mvc.Filters;
using Project.Core.Attributes;
using Project.Core.Interfaces;

namespace Project.WebApi.Filters;

/// <summary>
/// Factory for creating PermissionAuthorizationFilter instances.
/// This is needed because filters with dependencies must be created via a factory.
/// </summary>
public class PermissionAuthorizationFilterFactory : IFilterFactory
{
    private readonly Project.Core.Attributes.RequiresPermissionAttribute _attribute;

    public PermissionAuthorizationFilterFactory(Project.Core.Attributes.RequiresPermissionAttribute attribute)
    {
        _attribute = attribute;
    }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var permissionChecker = (IPermissionChecker)serviceProvider.GetService(typeof(IPermissionChecker))!;
        return new PermissionAuthorizationFilter(permissionChecker, _attribute);
    }
}
