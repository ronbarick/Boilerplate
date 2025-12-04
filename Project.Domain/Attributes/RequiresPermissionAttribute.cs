namespace Project.Domain.Attributes;

/// <summary>
/// Attribute to specify required permissions for AppService methods.
/// Can be applied at class or method level.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequiresPermissionAttribute : Attribute
{
    /// <summary>
    /// A list of permissions to authorize.
    /// </summary>
    public string[] Permissions { get; }

    /// <summary>
    /// If this property is set to true, all of the Permissions must be granted.
    /// If it's false, at least one of the Permissions must be granted.
    /// Default: false.
    /// </summary>
    public bool RequireAllPermissions { get; set; }

    /// <summary>
    /// Creates a new instance of RequiresPermissionAttribute.
    /// </summary>
    /// <param name="permissions">A list of permissions to authorize</param>
    public RequiresPermissionAttribute(params string[] permissions)
    {
        Permissions = permissions;
    }
}
