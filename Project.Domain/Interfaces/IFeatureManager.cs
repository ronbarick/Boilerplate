using Project.Domain.Entities.SaaS;


namespace Project.Domain.Interfaces;

public interface IFeatureManager
{
    Task SetPlanFeatureAsync(Guid planId, string featureName, string value);
    Task SetTenantFeatureAsync(Guid tenantId, string featureName, string value);
    Task<string> GetFeatureValueAsync(Guid tenantId, string featureName);
    Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureName);
    Task TrackFeatureUsageAsync(Guid tenantId, string featureName, int quantity = 1);
    Task ResetFeatureUsageAsync(Guid tenantId, string featureName);
    Task CheckUsageAlertsAsync();
}
