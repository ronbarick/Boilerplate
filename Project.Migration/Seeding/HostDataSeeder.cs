using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Core.Entities;
using Project.Infrastructure.Data;

namespace Project.Migration.Seeding;

public static class HostDataSeeder
{
    public static void SeedHostData(AppDbContext context, IConfiguration configuration)
    {
        // 0. Seed Settings
        SettingDataSeeder.SeedSettings(context, configuration);

        // Seed only if no host admin exists
        var hostAdmin = context.Users
            .IgnoreQueryFilters()
            .FirstOrDefault(u => u.TenantId == null && u.UserName == "admin");

        if (hostAdmin != null)
        {
            return; // Already seeded
        }

        // 1. Create host admin role (TenantId = null)
        var adminRole = context.Roles.FirstOrDefault(r => r.TenantId == null && r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = null, // Host role
                Name = "Admin",
                DisplayName = "Administrator",
                Description = "Host administrator role",
                IsStatic = true,
                IsDefault = true,
                CreatedOn = DateTime.UtcNow
            };
            context.Roles.Add(adminRole);
            context.SaveChanges();
        }

        // 4. Grant all permissions to host admin role
        var allPermissions = context.PermissionDefinitions.ToList();
        foreach (var permission in allPermissions)
        {
            if (!context.RolePermissions.Any(rp =>
                rp.RoleId == adminRole.Id && rp.PermissionName == permission.Name))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    TenantId = null, // Host permission
                    RoleId = adminRole.Id,
                    PermissionName = permission.Name,
                    IsGranted = true,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }

        context.SaveChanges();

        // 5. Create host admin user
        var adminUser = context.Users.FirstOrDefault(u => u.TenantId == null && u.UserName == "admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                TenantId = null, // Host user
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                EmailAddress = "admin@host.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedOn = DateTime.UtcNow
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
        }

        // 6. Assign admin role to admin user
        if (!context.UserRoles.Any(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
        {
            context.UserRoles.Add(new UserRole
            {
                Id = Guid.NewGuid(),
                TenantId = null,
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                CreatedOn = DateTime.UtcNow
            });
            context.SaveChanges();
        }
    }

}
