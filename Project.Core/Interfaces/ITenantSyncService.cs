using Project.Core.Entities.SaaS;

namespace Project.Core.Interfaces;

public interface ITenantSyncService
{
    Task SyncSubscriptionAsync(SaasTenantSubscription subscription);
    Task SyncFeatureAsync(Guid tenantId, string featureName, string value);
    Task SyncTenantDetailsAsync(Guid tenantId);
}
