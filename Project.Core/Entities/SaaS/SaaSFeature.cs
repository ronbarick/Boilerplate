using Project.Core.Entities.Base;
using Project.Core.Enums;

namespace Project.Core.Entities.SaaS;

/// <summary>
/// Global feature definition
/// </summary>
public class SaaSFeature : FullAuditedEntity
{
    /// <summary>
    /// Unique feature name (e.g., "MaxProjects", "AdvancedReporting")
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Display name for UI
    /// </summary>
    public string DisplayName { get; set; } = null!;
    
    /// <summary>
    /// Feature description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Default value for this feature
    /// </summary>
    public string DefaultValue { get; set; } = null!;
    
    /// <summary>
    /// Feature type
    /// </summary>
    public FeatureType FeatureType { get; set; }
    
    /// <summary>
    /// Feature group for organization (e.g., "Projects", "Reports")
    /// </summary>
    public string? GroupName { get; set; }
    
    /// <summary>
    /// Alert threshold percentage (e.g., 80 = alert at 80% usage)
    /// </summary>
    public int? AlertThresholdPercentage { get; set; }
}
