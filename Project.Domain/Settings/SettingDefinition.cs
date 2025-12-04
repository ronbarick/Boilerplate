namespace Project.Domain.Settings;

/// <summary>
/// Represents a setting definition (metadata).
/// </summary>
public class SettingDefinition
{
    /// <summary>
    /// Unique name of the setting
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Default value of the setting
    /// </summary>
    public string? DefaultValue { get; }

    /// <summary>
    /// Scopes where this setting can be defined
    /// </summary>
    public SettingScopes Scopes { get; }

    /// <summary>
    /// Display name for UI
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Description for UI
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether this setting is encrypted in the database
    /// </summary>
    public bool IsEncrypted { get; set; }

    public SettingDefinition(
        string name,
        string? defaultValue = null,
        SettingScopes scopes = SettingScopes.All,
        string? displayName = null,
        string? description = null,
        bool isEncrypted = false)
    {
        Name = name;
        DefaultValue = defaultValue;
        Scopes = scopes;
        DisplayName = displayName ?? name;
        Description = description;
        IsEncrypted = isEncrypted;
    }
}
