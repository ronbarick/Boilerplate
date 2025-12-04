using Project.Domain.Entities.SaaS;

namespace Project.Domain.Interfaces;

public interface ITenantSyncService
{
    Task SyncSubscriptionAsync(SaasTenantSubscription subscription);
    Task SyncFeatureAsync(Guid tenantId, string featureName, string value);
    Task SyncTenantDetailsAsync(Guid tenantId);
}
