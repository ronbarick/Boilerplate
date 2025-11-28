using Hangfire.Dashboard;

namespace Project.WebApi.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // Allow access only to authenticated users
        // You can add more sophisticated authorization logic here
        // For example, check for specific roles or permissions
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
