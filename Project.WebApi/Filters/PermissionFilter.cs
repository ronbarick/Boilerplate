using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Project.Domain.Interfaces;

namespace Project.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequiresPermissionAttribute : Attribute
{
    public string PermissionName { get; }

    public RequiresPermissionAttribute(string permissionName)
    {
        PermissionName = permissionName;
    }
}

public class PermissionFilter
{
    public static async Task<IResult> CheckPermissionAsync(
        HttpContext context,
        IPermissionChecker permissionChecker,
        string permissionName)
    {
        if (!await permissionChecker.IsGrantedAsync(context.User, permissionName))
        {
            return Results.StatusCode(403); // Forbidden
        }

        return null; // Permission granted, continue
    }
}
