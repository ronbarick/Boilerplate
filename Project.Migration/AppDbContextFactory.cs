using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Project.Core.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Migration;

/// <summary>
/// Design-time factory for creating AppDbContext instances during migrations.
/// This is required for EF Core tools to work without running the full application.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=ProjectDb;Username=postgres;Password=postgres";

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsAssembly("Project.Migration");
        });

        // Create dummy services for design-time
        var currentTenant = new DesignTimeCurrentTenant();
        var currentUser = new DesignTimeCurrentUser();
        var auditContext = new DesignTimeAuditContext();

        return new AppDbContext(optionsBuilder.Options, currentTenant, currentUser, auditContext);
    }

    /// <summary>
    /// Design-time implementation of ICurrentTenant (no tenant context during migrations)
    /// </summary>
    private class DesignTimeCurrentTenant : ICurrentTenant
    {
        public Guid? Id => null;
        public string? Name => null;
        public bool IsAvailable => false;
    }

    /// <summary>
    /// Design-time implementation of ICurrentUser (no user context during migrations)
    /// </summary>
    private class DesignTimeCurrentUser : ICurrentUser
    {
        public Guid? Id => null;
        public string? UserName => null;
        public string? Email => null;
        public Guid? TenantId => null;
        public bool IsAuthenticated => false;
        public bool IsInRole(string roleName) => false;
    }

    /// <summary>
    /// Design-time implementation of IAuditContext (no audit context during migrations)
    /// </summary>
    private class DesignTimeAuditContext : IAuditContext
    {
        public string? ServiceName { get; set; }
        public string? MethodName { get; set; }
        public string? ClientIpAddress { get; set; }
        public string? BrowserInfo { get; set; }
    }
}
