using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace Project.WebApi.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // Try to resolve tenant from:
        // 1. Header: X-Tenant-Id or X-Tenant-Name
        // 2. Subdomain (optional, not implemented here)
        // 3. User claims (if authenticated)

        Guid? tenantId = null;
        string? tenantName = null;

        // Check header for Tenant ID
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader.FirstOrDefault(), out var parsedTenantId))
            {
                tenantId = parsedTenantId;
            }
        }

        // Check header for Tenant Name
        if (!tenantId.HasValue && context.Request.Headers.TryGetValue("X-Tenant-Name", out var tenantNameHeader))
        {
            tenantName = tenantNameHeader.FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantName))
            {
                var tenant = await dbContext.Tenants
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.Name == tenantName);
                
                if (tenant != null)
                {
                    tenantId = tenant.Id;
                }
            }
        }

        // Check user claims (if authenticated)
        if (!tenantId.HasValue && context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("TenantId");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var claimTenantId))
            {
                tenantId = claimTenantId;
            }
        }

        // Store in HttpContext.Items for CurrentTenant service
        if (tenantId.HasValue)
        {
            context.Items["TenantId"] = tenantId.Value;
            
            // Optionally fetch tenant name if not already set
            if (string.IsNullOrEmpty(tenantName))
            {
                var tenant = await dbContext.Tenants
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.Id == tenantId.Value);
                
                if (tenant != null)
                {
                    context.Items["TenantName"] = tenant.Name;
                }
            }
            else
            {
                context.Items["TenantName"] = tenantName;
            }
        }

        await _next(context);
    }
}
