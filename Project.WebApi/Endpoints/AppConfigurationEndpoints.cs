using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Project.Application.AppConfiguration;
using Project.Application.AppConfiguration.Dtos;
using System.Threading.Tasks;

namespace Project.WebApi.Endpoints;

public static class AppConfigurationEndpoints
{
    public static void MapAppConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app").WithTags("Application");

        // Before & After Login - Application Configuration
        group.MapGet("/application-configuration", GetApplicationConfigurationAsync)
            .AllowAnonymous()
            .CacheOutput(policy => policy
                .Expire(TimeSpan.FromMinutes(5))
                .Tag("app-config"))
            .WithName("GetApplicationConfiguration");

        // Before Login - Localization Texts
        group.MapGet("/localization", GetLocalizationTextsAsync)
            .AllowAnonymous()
            .CacheOutput(policy => policy
                .Expire(TimeSpan.FromMinutes(10))
                .Tag("localization")
                .SetVaryByQuery("culture", "resourceName"))
            .WithName("GetLocalizationTexts");

        // Before Login - Tenant by Name
        group.MapGet("/tenants/by-name/{name}", GetTenantByNameAsync)
            .AllowAnonymous()
            .WithName("GetTenantByName");

        // After Login - My Profile
        group.MapGet("/my-profile", GetMyProfileAsync)
            .RequireAuthorization()
            .WithName("GetMyProfile");

        // After Login - Permissions Tree
        group.MapGet("/permissions", GetPermissionsAsync)
            .RequireAuthorization()
            .WithName("GetPermissions");

        // After Login - Features List
        group.MapGet("/features", GetFeaturesAsync)
            .RequireAuthorization()
            .WithName("GetFeatures");

        // After Login - Settings
        group.MapGet("/settings", GetSettingsAsync)
            .RequireAuthorization()
            .WithName("GetSettings");
    }

    private static async Task<IResult> GetApplicationConfigurationAsync(
        IApplicationConfigurationAppService appConfigService)
    {
        var config = await appConfigService.GetAsync();
        return Results.Ok(config);
    }

    private static async Task<IResult> GetLocalizationTextsAsync(
        string culture,
        string? resourceName,
        ILocalizationAppService localizationService)
    {
        var texts = await localizationService.GetTextsAsync(culture, resourceName);
        return Results.Ok(texts);
    }

    private static async Task<IResult> GetTenantByNameAsync(
        string name,
        Project.Application.Tenants.ITenantAppService tenantService)
    {
        var tenant = await tenantService.GetByNameAsync(name);
        if (tenant == null)
            return Results.NotFound(new { message = "Tenant not found or inactive" });

        return Results.Ok(new
        {
            success = true,
            tenantId = tenant.Id,
            name = tenant.Name,
            isActive = tenant.IsActive
        });
    }

    private static async Task<IResult> GetMyProfileAsync(
        IProfileAppService profileService)
    {
        try
        {
            var profile = await profileService.GetMyProfileAsync();
            return Results.Ok(profile);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> GetPermissionsAsync(
        IApplicationConfigurationAppService appConfigService)
    {
        var config = await appConfigService.GetAsync();
        
        // Return granted policies as a simple dictionary
        return Results.Ok(new { grantedPolicies = config.Auth.GrantedPolicies });
    }

    private static async Task<IResult> GetFeaturesAsync(
        IApplicationConfigurationAppService appConfigService)
    {
        var config = await appConfigService.GetAsync();
        return Results.Ok(new { features = config.Features.Values });
    }

    private static async Task<IResult> GetSettingsAsync(
        IApplicationConfigurationAppService appConfigService)
    {
        var config = await appConfigService.GetAsync();
        return Results.Ok(new { settings = config.Settings });
    }
}
