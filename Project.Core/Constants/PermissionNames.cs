namespace Project.Core.Constants;

/// <summary>
/// Permission names used throughout the application.
/// Following ABP Framework pattern for permission definition.
/// </summary>
public static partial class PermissionNames
{
    // Tenant Management (Host Only)
    public const string Pages_Tenants = "Pages.Tenants";
    public const string Pages_Tenants_Create = "Pages.Tenants.Create";
    public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
    public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";

    // User Management
    public const string Pages_Users = "Pages.Users";
    public const string Pages_Users_Create = "Pages.Users.Create";
    public const string Pages_Users_Edit = "Pages.Users.Edit";
    public const string Pages_Users_Delete = "Pages.Users.Delete";
    public const string Pages_Users_ChangePermissions = "Pages.Users.ChangePermissions";

    // Role Management
    public const string Pages_Roles = "Pages.Roles";
    public const string Pages_Roles_Create = "Pages.Roles.Create";
    public const string Pages_Roles_Edit = "Pages.Roles.Edit";
    public const string Pages_Roles_Delete = "Pages.Roles.Delete";

    // Student Management
    public const string Pages_Students = "Pages.Students";
    public const string Pages_Students_Create = "Pages.Students.Create";
    public const string Pages_Students_Edit = "Pages.Students.Edit";
    public const string Pages_Students_Delete = "Pages.Students.Delete";
    // Reporting
    public const string Reports_Generate = "Reports.Generate";
    public const string Reports_Download = "Reports.Download";
    public const string Reports_BackgroundJobs = "Reports.BackgroundJobs";
}
