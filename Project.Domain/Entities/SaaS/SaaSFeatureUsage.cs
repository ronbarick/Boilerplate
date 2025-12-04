using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.SaaS;

/// <summary>
/// Feature usage tracking for metered/numeric features
/// </summary>
public class SaaSFeatureUsage : FullAuditedEntity
{
    /// <summary>
    /// Feature name
    /// </summary>
    public string FeatureName { get; set; } = null!;
    
    /// <summary>
    /// Current usage count
    /// </summary>
    public int UsageCount { get; set; }
    
    /// <summary>
    /// Usage month (first day of month)
    /// </summary>
    public DateTime UsageMonth { get; set; }
    
    /// <summary>
    /// Whether usage alert has been sent
    /// </summary>
    public bool AlertSent { get; set; }
}
