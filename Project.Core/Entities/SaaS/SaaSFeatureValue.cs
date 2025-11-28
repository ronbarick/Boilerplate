using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Feature value override for tenant or plan
/// </summary>
public class SaaSFeatureValue : FullAuditedEntity
{
    /// <summary>
    /// Entity type (Tenant or Plan)
    /// </summary>
    public FeatureEntityType EntityType { get; set; }
    
    /// <summary>
    /// Entity ID (TenantId or PlanId)
    /// </summary>
    public Guid EntityId { get; set; }
    
    /// <summary>
    /// Feature name
    /// </summary>
    public string FeatureName { get; set; } = null!;
    
    /// <summary>
    /// Feature value
    /// </summary>
    public string FeatureValue { get; set; } = null!;
}
