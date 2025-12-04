
using Project.Domain.Interfaces;

namespace Project.Domain.Authorization;

/// <summary>
/// Defines all permissions for the Project application.
/// </summary>
public class ProjectAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        // ===== TENANT MANAGEMENT (Host Only) =====
        var tenants = context.CreatePermission(
            PermissionNames.Pages_Tenants,
            "Tenant Management",
            isHostOnly: true);

        tenants.CreateChildPermission(
            PermissionNames.Pages_Tenants_Create,
            "Create Tenant");

        tenants.CreateChildPermission(
            PermissionNames.Pages_Tenants_Edit,
            "Edit Tenant");

        tenants.CreateChildPermission(
            PermissionNames.Pages_Tenants_Delete,
            "Delete Tenant");

        // ===== USER MANAGEMENT =====
        var users = context.CreatePermission(
            PermissionNames.Pages_Users,
            "User Management");

        users.CreateChildPermission(
            PermissionNames.Pages_Users_Create,
            "Create User");

        users.CreateChildPermission(
            PermissionNames.Pages_Users_Edit,
            "Edit User");

        users.CreateChildPermission(
            PermissionNames.Pages_Users_Delete,
            "Delete User");

        users.CreateChildPermission(
            PermissionNames.Pages_Users_ChangePermissions,
            "Change User Permissions");

        // ===== ROLE MANAGEMENT =====
        var roles = context.CreatePermission(
            PermissionNames.Pages_Roles,
            "Role Management");

        roles.CreateChildPermission(
            PermissionNames.Pages_Roles_Create,
            "Create Role");

        roles.CreateChildPermission(
            PermissionNames.Pages_Roles_Edit,
            "Edit Role");

        roles.CreateChildPermission(
            PermissionNames.Pages_Roles_Delete,
            "Delete Role");

        // ===== STUDENT MANAGEMENT =====
        var students = context.CreatePermission(
            PermissionNames.Pages_Students,
            "Student Management");

        students.CreateChildPermission(
            PermissionNames.Pages_Students_Create,
            "Create Student");

        students.CreateChildPermission(
            PermissionNames.Pages_Students_Edit,
            "Edit Student");

        students.CreateChildPermission(
            PermissionNames.Pages_Students_Delete,
            "Delete Student");

        // ===== FILE STORAGE MANAGEMENT =====
        var fileStorage = context.CreatePermission(
            PermissionNames.Pages_FileStorage,
            "File Storage Management");

        fileStorage.CreateChildPermission(
            PermissionNames.Pages_FileStorage_Upload,
            "Upload Files");

        fileStorage.CreateChildPermission(
            PermissionNames.Pages_FileStorage_Download,
            "Download Files");

        fileStorage.CreateChildPermission(
            PermissionNames.Pages_FileStorage_Delete,
            "Delete Files");

        fileStorage.CreateChildPermission(
            PermissionNames.Pages_FileStorage_Admin,
            "File Storage Administration");
    }
}
