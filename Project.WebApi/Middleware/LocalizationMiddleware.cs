using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Project.Domain.Localization;
using System.Globalization;

namespace Project.WebApi.Middleware;

/// <summary>
/// Middleware to resolve and set the current culture for each request.
/// </summary>
public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICultureProvider cultureProvider)
    {
        try
        {
            // Resolve culture from multiple sources
            var cultureName = await cultureProvider.GetCurrentCultureAsync();

            // Set thread culture
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            // Set culture in response cookie for future requests
            SetCultureCookie(context, cultureName);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the request
            // Fall back to default culture
            var defaultCulture = new CultureInfo("en");
            CultureInfo.CurrentCulture = defaultCulture;
            CultureInfo.CurrentUICulture = defaultCulture;
        }

        await _next(context);
    }

    private void SetCultureCookie(HttpContext context, string cultureName)
    {
        // Set culture cookie in the format expected by ASP.NET Core
        var cookieValue = $"c={cultureName}|uic={cultureName}";
        
        context.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
    }
}
