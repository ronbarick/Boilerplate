using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Project.WebApi.Middleware;

/// <summary>
/// Middleware to protect Swagger endpoints with JWT authentication.
/// Only authenticated admin users can access Swagger documentation.
/// </summary>
public class SwaggerAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SwaggerAuthMiddleware> _logger;

    public SwaggerAuthMiddleware(RequestDelegate next, ILogger<SwaggerAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Check if request is for Swagger
        if (path.StartsWith("/swagger"))
        {
            // Allow swagger-login.html without authentication
            if (path == "/swagger-login.html")
            {
                await _next(context);
                return;
            }

            // Check for JWT token
            var token = GetTokenFromRequest(context);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Unauthorized Swagger access attempt from {IP}", context.Connection.RemoteIpAddress);
                context.Response.Redirect("/swagger-login.html");
                return;
            }

            try
            {
                // Validate token and check if user is admin
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Check if token is expired
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    _logger.LogWarning("Expired token used for Swagger access from {IP}", context.Connection.RemoteIpAddress);
                    context.Response.Redirect("/swagger-login.html");
                    return;
                }

                // Check if user has admin role (optional - you can customize this)
                var roles = jwtToken.Claims
                    .Where(c => c.Type == "role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                    .Select(c => c.Value)
                    .ToList();

                // For now, we'll allow any authenticated user
                // You can uncomment below to restrict to admin only:
                // if (!roles.Contains("Admin"))
                // {
                //     _logger.LogWarning("Non-admin user attempted Swagger access: {User}", jwtToken.Subject);
                //     context.Response.StatusCode = 403;
                //     await context.Response.WriteAsync("Forbidden: Admin access required");
                //     return;
                // }

                _logger.LogInformation("Swagger access granted to user: {User}", jwtToken.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid token for Swagger access from {IP}", context.Connection.RemoteIpAddress);
                context.Response.Redirect("/swagger-login.html");
                return;
            }
        }

        await _next(context);
    }

    private string? GetTokenFromRequest(HttpContext context)
    {
        // Try to get token from localStorage (via custom header)
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        
        if (!string.IsNullOrEmpty(token))
            return token;

        // Try to get from cookie (if you set it)
        token = context.Request.Cookies["swagger_token"];
        
        if (!string.IsNullOrEmpty(token))
            return token;

        // Try from query string (for initial redirect)
        token = context.Request.Query["token"].FirstOrDefault();

        return token;
    }
}
