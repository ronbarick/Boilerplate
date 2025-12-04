using Microsoft.EntityFrameworkCore;
using Project.Domain.Shared.Constants;
using Project.Domain.Entities;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Data.Seeding;

/// <summary>
/// Seeds initial data for a new tenant.
/// Creates admin role and assigns all non-host permissions.
/// </summary>
public static class TenantDataSeeder
{
    public static void SeedTenantData(AppDbContext context, Guid tenantId)
    {
        // 1. Create tenant admin role
        var adminRole = context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Admin",
                DisplayName = "Administrator",
                Description = "Tenant administrator role",
                IsStatic = true,
                IsDefault = true,
                CreatedOn = DateTime.UtcNow
            };
            context.Roles.Add(adminRole);
            context.SaveChanges();
        }

        // 2. Get all non-host permissions
        var permissions = context.PermissionDefinitions
            .IgnoreQueryFilters()
            .Where(p => !p.IsHost)
            .ToList();

        // 3. Grant all tenant permissions to admin role
        foreach (var permission in permissions)
        {
            if (!context.RolePermissions.Any(rp =>
                rp.RoleId == adminRole.Id && rp.PermissionName == permission.Name))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RoleId = adminRole.Id,
                    PermissionName = permission.Name,
                    IsGranted = true,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }

        context.SaveChanges();
    }
}
