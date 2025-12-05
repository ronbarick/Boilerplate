using Project.WebApi.Endpoints;
using Project.WebApi.Extensions;
using Project.WebApi.Middleware;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.PostgreSql;
using Project.Infrastructure.Sms;
using Project.Domain.Interfaces;
using Project.Infrastructure.Services;
using Project.WebApi.ExceptionHandling;
using Project.Application.BackgroundJobs.Students;
using Project.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); // Required for SMS senders

// Database
builder.Services.AddDatabase(builder.Configuration);

// Core Services (Current User/Tenant, Permissions, Repositories, AutoMapper, Infrastructure, FluentValidation)
builder.Services.AddCoreServices();

// Application Services - Automatically register all IApplicationService implementations
builder.Services.AddApplicationServices(typeof(Project.Application.Users.UserAppService).Assembly);

builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

// SMS Services
builder.Services.AddScoped<TwilioSmsSender>();
builder.Services.AddScoped<Msg91SmsSender>();
builder.Services.AddScoped<SmsSenderFactory>();

// Exception Handling
builder.Services.AddScoped<IExceptionToErrorInfoConverter, SeverityErrorInfoConverter>();



// JWT Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Production Security (CORS, Rate Limiting, HSTS)
builder.Services.AddProductionSecurity(builder.Configuration);

// Dynamic API Controllers
builder.Services.AddDynamicApiControllers(typeof(Project.Application.Users.UserAppService).Assembly);



// Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    }));

builder.Services.AddHangfireServer();

// SignalR
builder.Services.AddSignalR();

// Swagger/OpenAPI
builder.Services.AddSwaggerWithJwt();

// Output Caching (required by CacheInvalidationService)
builder.Services.AddOutputCache();

var app = builder.Build();

// Migrate and Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Project.Infrastructure.Data.AppDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();

        // Apply migrations
        context.Database.Migrate();

        // Seed data
        Project.Migration.Seeding.HostDataSeeder.SeedHostData(context, configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HTTPS Strict Transport Security
}

// Global Exception Logging
app.UseMiddleware<GlobalExceptionMiddleware>();

// Security Headers
app.UseSecurityHeaders();

app.UseHttpsRedirection();

// Default Files (serves index.html on root URL)
app.UseDefaultFiles();

// Static Files (for swagger-login.html and swagger-auth.js)
app.UseStaticFiles();

// Rate Limiting
app.UseIpRateLimiting();

// Swagger (protected by auth middleware)
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // Swagger Auth Middleware - MUST be before UseSwagger
    app.UseMiddleware<SwaggerAuthMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";

        // Inject JWT token from localStorage into Swagger requests
        c.InjectJavascript("/swagger-auth.js");

        // Inject Dark Theme
        c.InjectStylesheet("/swagger-dark.css");
    });
}

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// CORS
app.UseCors();

app.UseAuthentication();

// Localization - MUST be after UseAuthentication so context.User is populated
app.UseMiddleware<LocalizationMiddleware>();

// Custom Middleware - MUST be after UseAuthentication so context.User is populated
app.UseMiddleware<TenantResolverMiddleware>();
app.UseMiddleware<AuditMiddleware>();

app.UseAuthorization();

// Map Controllers (auto-generated from AppServices)
app.MapControllers();

// Map Auth Endpoints (keep authentication separate)
app.MapAuthEndpoints();

// Map User Endpoints (password management and email confirmation)
app.MapUserEndpoints();



// Map SignalR Hub
app.MapHub<Project.WebApi.Hubs.NotificationHub>("/notification-hub");

// Configure Recurring Jobs
ConfigureRecurringJobs(app.Services);

app.Run();

static void ConfigureRecurringJobs(IServiceProvider services)
{
    var recurringJobManager = services.GetRequiredService<Hangfire.IRecurringJobManager>();

    // Student List Job - Runs daily at 10 PM
    recurringJobManager.AddOrUpdate<StudentListJob>(
        "student-list-daily",
        job => job.ExecuteAsync(),
        "0 22 * * *"); // Cron: Every day at 22:00 (10 PM)

    // Two-Factor Cleanup Job - Runs hourly (commented out - job class doesn't exist)
    // recurringJobManager.AddOrUpdate<TwoFactorCleanupJob>(
    //     "two-factor-cleanup",
    //     job => job.ExecuteAsync(),
    //     Cron.Hourly);
}
