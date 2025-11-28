using Project.Core.Entities.Base;

namespace Project.Core.Entities;

/// <summary>
/// Represents a setting value stored in the database.
/// Settings follow a hierarchical resolution: User → Tenant → Global → Default
/// </summary>
public class Setting : FullAuditedEntity
{
    /// <summary>
    /// Setting name (e.g., "App.Theme", "App.Language")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Setting value (stored as string, can be parsed to appropriate type)
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Provider name: "G" (Global), "T" (Tenant), "U" (User)
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// Provider key: null for Global, TenantId for Tenant, UserId for User
    /// </summary>
    public string? ProviderKey { get; set; }
}
