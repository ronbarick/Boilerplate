using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using Project.Domain.Interfaces;

namespace Project.Application.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresSubscriptionFeatureAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var settingProvider = context.HttpContext.RequestServices.GetRequiredService<ISettingProvider>();

        // Check Global Setting
        var isGloballyEnabled = await settingProvider.GetOrNullAsync(SaaSSettingNames.IsEnabled);
        if (isGloballyEnabled?.ToLower() == "false")
        {
            context.Result = new NotFoundResult();
            return;
        }

        // Check Tenant Setting
        var isTenantEnabled = await settingProvider.GetOrNullAsync(SaaSSettingNames.TenantIsEnabled);
        if (isTenantEnabled?.ToLower() == "false")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        await next();
    }
}
