using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Project.WebApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context, 
        IRepository<AuditLog> auditRepository,
        IAuditContext auditContext,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        Project.WebApi.ExceptionHandling.IExceptionToErrorInfoConverter converter,
        Microsoft.Extensions.Hosting.IHostEnvironment env)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            try
            {
                await auditRepository.InsertAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = currentUser.Id,
                    TenantId = currentTenant.Id,
                    ExecutionTime = DateTime.UtcNow,
                    ServiceName = auditContext.ServiceName ?? "GlobalExceptionMiddleware",
                    MethodName = auditContext.MethodName ?? "InvokeAsync",
                    ClientIpAddress = auditContext.ClientIpAddress,
                    BrowserInfo = auditContext.BrowserInfo,
                    Exception = ex.ToString(),
                    CustomData = "Internal Server Error"
                });
            }
            catch
            {
                // If logging fails, we don't want to hide the original exception
                // Just suppress the logging error
            }

            // Handle exception response
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            
            if (ex is Project.Core.Exceptions.UserFriendlyException)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden; // Or 400 Bad Request, usually 403 for business rules or 400
                // ABP uses 403 for UserFriendlyException usually, or 400. Let's use 400 for general validation/business errors if not specified.
                // But UserFriendlyException is often 403. Let's stick to 403 or 400. 
                // Let's use 403 Forbidden as it's "User Friendly" meaning business rule violation.
                // Actually, often it's 403.
                context.Response.StatusCode = 403;
            }

            if (ex is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            }

            if (ex is DbUpdateConcurrencyException)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
            }

            context.Response.ContentType = "application/json";

            var errorInfo = converter.Convert(ex, env.IsDevelopment());
            var response = new Project.WebApi.ExceptionHandling.RemoteServiceErrorResponse(errorInfo);

            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsJsonAsync(response, jsonOptions);
        }
    }
}
