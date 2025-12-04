using Project.Domain.UnitOfWork;

namespace Project.WebApi.Middleware;

public class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWorkManager unitOfWorkManager)
    {
        if (IsIgnoredUrl(context))
        {
            await _next(context);
            return;
        }

        using (var uow = unitOfWorkManager.Begin())
        {
            await _next(context);
            await uow.CommitAsync();
        }
    }

    private bool IsIgnoredUrl(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        return path != null && (
            path.StartsWith("/swagger") ||
            path.StartsWith("/health") ||
            path.StartsWith("/hangfire")
        );
    }
}
