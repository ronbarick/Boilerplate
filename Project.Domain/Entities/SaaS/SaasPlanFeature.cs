using Project.Domain.Entities.Base;


namespace Project.Domain.Entities.SaaS;

/// <summary>
/// Feature assigned to a subscription plan
/// </summary>
public class SaasPlanFeature : FullAuditedEntity
{
    /// <summary>
    /// Plan ID
    /// </summary>
    public Guid PlanId { get; set; }
    
    /// <summary>
    /// Feature name (e.g., "MaxProjects", "AdvancedReporting")
    /// </summary>
    public string FeatureName { get; set; } = null!;
    
    /// <summary>
    /// Feature value (e.g., "100", "true", "unlimited")
    /// </summary>
    public string FeatureValue { get; set; } = null!;
    
    /// <summary>
    /// Feature type
    /// </summary>
    public FeatureType FeatureType { get; set; }
    
    /// <summary>
    /// Navigation property to plan
    /// </summary>
    public SaasPlan Plan { get; set; } = null!;
}
