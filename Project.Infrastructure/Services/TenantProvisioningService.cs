using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Services
{
    public class TenantProvisioningService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public TenantProvisioningService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task ProvisionTenantAsync(Tenant tenant, string adminPassword)
        {
            // Check if using shared database from configuration
            var useSharedDatabase = bool.TryParse(_configuration["MultiTenancy:UseSharedDatabase"], out var shared) && shared;
            var defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");

            AppDbContext dbContext;

            if (useSharedDatabase)
            {
                // Shared Database Mode: Use the host database
                // Set tenant connection string to null to indicate shared database
                tenant.ConnectionString = null;

                // Get the host's DbContext from DI (already configured with DefaultConnection)
                using var scope = _serviceProvider.CreateScope();
                dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // No need to migrate - host database is already migrated
                // Just seed tenant-specific data
                await SeedTenantDataAsync(dbContext, tenant.Id, adminPassword);
            }
            else
            {
                // Separate Database Mode: Create a new database for the tenant
                var tenantConnectionString = tenant.ConnectionString ?? BuildConnectionString(defaultConnectionString, tenant.TenancyName);
                tenant.ConnectionString = tenantConnectionString;

                // Create DB and Apply Migrations
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseNpgsql(tenantConnectionString, b => b.MigrationsAssembly("Project.Migration"));

                using var scope = _serviceProvider.CreateScope();
                
                dbContext = new AppDbContext(
                    optionsBuilder.Options, 
                    new MockCurrentTenant(tenant.Id, tenant.Name), 
                    new MockCurrentUser(),
                    new MockAuditContext());

                await dbContext.Database.MigrateAsync();

                // Seed Data
                await SeedTenantDataAsync(dbContext, tenant.Id, adminPassword);
            }
        }

        private string BuildConnectionString(string defaultConnection, string tenancyName)
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(defaultConnection);
            builder.Database = $"{builder.Database}_{tenancyName}";
            return builder.ToString();
        }

        private async Task SeedTenantDataAsync(AppDbContext context, Guid tenantId, string adminPassword)
        {
            // 1. Seed Roles and Permissions using the centralized seeder
            Project.Infrastructure.Data.Seeding.TenantDataSeeder.SeedTenantData(context, tenantId);
            
            // IMPORTANT: Save changes from seeder before continuing
            await context.SaveChangesAsync();

            // 2. Seed User Role (if not created by seeder - seeder only creates Admin)
            var userRoleId = Guid.NewGuid();
            var userRole = new Role
            {
                Id = userRoleId,
                Name = "User",
                DisplayName = "User",
                Description = "Standard user role",
                IsStatic = true,
                IsDefault = true,
                TenantId = tenantId,
                CreatedOn = DateTime.UtcNow
            };
            
            if (!await context.Roles.AnyAsync(r => r.TenantId == tenantId && r.Name == "User"))
            {
                context.Roles.Add(userRole);
            }

            // 3. Seed Tenant Admin User
            var adminUserId = Guid.NewGuid();
            var adminUser = new User
            {
                Id = adminUserId,
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                EmailAddress = "admin@tenant.com",
                TenantId = tenantId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedOn = DateTime.UtcNow
            };
            context.Users.Add(adminUser);

            // 4. Assign Admin Role to Admin User
            // First check local context, then database
            var adminRole = context.Roles.Local.FirstOrDefault(r => r.TenantId == tenantId && r.Name == "Admin")
                ?? await context.Roles.FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Name == "Admin");
            
            if (adminRole == null)
            {
                throw new InvalidOperationException($"Admin role not found for tenant {tenantId}. Ensure TenantDataSeeder.SeedTenantData was called successfully.");
            }
            
            var userRoleAssignment = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = adminUserId,
                RoleId = adminRole.Id,
                TenantId = tenantId,
                CreatedOn = DateTime.UtcNow
            };
            context.UserRoles.Add(userRoleAssignment);

            await context.SaveChangesAsync();
        }
    }

    // Mocks for provisioning
    internal class MockCurrentTenant : ICurrentTenant
    {
        public Guid? Id { get; }
        public string? Name { get; }
        public bool IsAvailable => true;

        public MockCurrentTenant(Guid? id, string? name)
        {
            Id = id;
            Name = name;
        }
    }

    internal class MockCurrentUser : ICurrentUser
    {
        public Guid? Id => null;
        public string? UserName => "System";
        public string? Email => null;
        public Guid? TenantId => null;
        public bool IsAuthenticated => true;
        public bool IsInRole(string roleName) => true;
    }

    internal class MockAuditContext : IAuditContext
    {
        public string? ServiceName { get; set; }
        public string? MethodName { get; set; }
        public string? ClientIpAddress { get; set; }
        public string? BrowserInfo { get; set; }
    }
}
