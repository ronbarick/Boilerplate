using Microsoft.AspNetCore.Builder;

namespace Project.WebApi.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Remove server header
        context.Response.Headers.Remove("Server");
        
        // X-Content-Type-Options: prevents MIME-sniffing
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        
        // X-Frame-Options: prevents clickjacking
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        
        // X-XSS-Protection: enables cross-site scripting filter
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        
        // Referrer-Policy: controls referrer information
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        
        // Content-Security-Policy: prevents XSS and other attacks
        context.Response.Headers.Append("Content-Security-Policy", 
            "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline';");
        
        // Permissions-Policy: controls browser features
        context.Response.Headers.Append("Permissions-Policy", 
            "geolocation=(), microphone=(), camera=()");

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
