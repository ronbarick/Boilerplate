using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
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
