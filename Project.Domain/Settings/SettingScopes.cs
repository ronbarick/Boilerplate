namespace Project.Domain.Settings;

/// <summary>
/// Defines the scopes where a setting can be defined.
/// </summary>
[Flags]
public enum SettingScopes
{
    /// <summary>
    /// Global setting (system-wide)
    /// </summary>
    Global = 1,

    /// <summary>
    /// Tenant-specific setting
    /// </summary>
    Tenant = 2,

    /// <summary>
    /// User-specific setting
    /// </summary>
    User = 4,

    /// <summary>
    /// All scopes
    /// </summary>
    All = Global | Tenant | User
}
