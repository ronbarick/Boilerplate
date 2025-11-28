using Microsoft.AspNetCore.Mvc.Filters;
using Project.Core.Attributes;
using Project.Core.Interfaces;

namespace Project.WebApi.Filters;

/// <summary>
/// Authorization filter that checks permissions specified by RequiresPermissionAttribute.
/// </summary>
public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IPermissionChecker _permissionChecker;
    private readonly Project.Core.Attributes.RequiresPermissionAttribute _attribute;

    public PermissionAuthorizationFilter(IPermissionChecker permissionChecker, Project.Core.Attributes.RequiresPermissionAttribute attribute)
    {
        _permissionChecker = permissionChecker;
        _attribute = attribute;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (_attribute.RequireAllPermissions)
        {
            // All permissions must be granted
            foreach (var permission in _attribute.Permissions)
            {
                if (!await _permissionChecker.IsGrantedAsync(user, permission))
                {
                    context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
                    return;
                }
            }
        }
        else
        {
            // At least one permission must be granted
            var hasAnyPermission = false;
            foreach (var permission in _attribute.Permissions)
            {
                if (await _permissionChecker.IsGrantedAsync(user, permission))
                {
                    hasAnyPermission = true;
                    break;
                }
            }

            if (!hasAnyPermission)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
            }
        }
    }
}
