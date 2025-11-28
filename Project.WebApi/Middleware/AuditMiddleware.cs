using Project.Core.Interfaces;

namespace Project.WebApi.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditContext auditContext)
    {
        // Capture Client IP
        auditContext.ClientIpAddress = context.Connection.RemoteIpAddress?.ToString();
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            auditContext.ClientIpAddress = context.Request.Headers["X-Forwarded-For"];
        }

        // Capture Browser Info (User-Agent)
        if (context.Request.Headers.ContainsKey("User-Agent"))
        {
            auditContext.BrowserInfo = context.Request.Headers["User-Agent"];
        }

        await _next(context);
    }
}
