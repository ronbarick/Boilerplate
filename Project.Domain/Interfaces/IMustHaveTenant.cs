namespace Project.Domain.Interfaces;

/// <summary>
/// Interface for entities that must have a tenant.
/// Entities implementing this interface cannot exist without a tenant context.
/// TenantId will be automatically assigned during creation and strict tenant filtering will be applied.
/// Note: TenantId is nullable at the C# level but will be required in the database via EF Core configuration.
/// </summary>
public interface IMustHaveTenant
{
    /// <summary>
    /// Gets or sets the tenant identifier. 
    /// This will be configured as required in the database.
    /// </summary>
    Guid? TenantId { get; set; }
}
