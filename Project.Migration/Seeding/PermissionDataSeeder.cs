using Microsoft.EntityFrameworkCore;
using Project.Domain.Authorization;
using Project.Domain.Entities;
using Project.Domain.Authorization;
using Project.Infrastructure.Data;

namespace Project.Migration.Seeding;

/// <summary>
/// Seeds and syncs permission definitions from code to the database.
/// </summary>
public static class PermissionDataSeeder
{
    public static void SeedPermissions(AppDbContext context)
    {
        // 1. Get permissions from code
        var authProvider = new ProjectAuthorizationProvider();
        var permissionContext = new PermissionDefinitionContext();
        authProvider.SetPermissions(permissionContext);
        var codePermissions = FlattenPermissions(permissionContext.Permissions);

        // 2. Get permissions from DB
        var dbPermissions = context.PermissionDefinitions.IgnoreQueryFilters().ToList();

        // 3. Sync (Insert/Update)
        foreach (var codePerm in codePermissions)
        {
            var dbPerm = dbPermissions.FirstOrDefault(p => p.Name == codePerm.Name);
            if (dbPerm == null)
            {
                // Insert new permission
                context.PermissionDefinitions.Add(new PermissionDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = codePerm.Name,
                    DisplayName = codePerm.DisplayName,
                    Description = codePerm.DisplayName,
                    ParentName = codePerm.Parent?.Name,
                    IsStatic = true,
                    IsHost = codePerm.IsHostOnly,
                    CreatedOn = DateTime.UtcNow
                });
            }
            else
            {
                // Update existing permission
                // Only update fields that might change in code
                if (dbPerm.DisplayName != codePerm.DisplayName ||
                    dbPerm.IsHost != codePerm.IsHostOnly ||
                    dbPerm.ParentName != codePerm.Parent?.Name)
                {
                    dbPerm.DisplayName = codePerm.DisplayName;
                    dbPerm.Description = codePerm.DisplayName; // Assuming description mimics display name for now
                    dbPerm.IsHost = codePerm.IsHostOnly;
                    dbPerm.ParentName = codePerm.Parent?.Name;
                    
                    // Mark as modified if tracked, though EF usually tracks changes automatically
                    context.Entry(dbPerm).State = EntityState.Modified;
                }
            }
        }

        context.SaveChanges();
    }

    private static List<Permission> FlattenPermissions(List<Permission> permissions)
    {
        var result = new List<Permission>();
        foreach (var permission in permissions)
        {
            result.Add(permission);
            result.AddRange(FlattenPermissions(permission.Children));
        }
        return result;
    }
}
