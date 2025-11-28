using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Core.Entities.SaaS;
using Project.Core.Enums;
using Project.Core.Interfaces;

namespace Project.Application.BackgroundJobs.SaaS;

public class SubscriptionRenewalWorker
{
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly ILogger<SubscriptionRenewalWorker> _logger;

    public SubscriptionRenewalWorker(
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        ILogger<SubscriptionRenewalWorker> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Checking for subscriptions due for renewal...");
        
        try
        {
            var today = DateTime.UtcNow.Date;
            
            var renewalDue = await _subscriptionRepository.GetQueryable()
                .Where(x => x.Status == SubscriptionStatus.Active 
                    && x.AutoRenew
                    && x.EndDate.HasValue
                    && x.EndDate.Value.Date == today)
                .ToListAsync();

            _logger.LogInformation($"Found {renewalDue.Count} subscriptions due for renewal.");
            
            foreach (var subscription in renewalDue)
            {
                _logger.LogInformation($"Processing renewal for subscription {subscription.Id}");
                // TODO: Trigger payment and extend subscription
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing subscription renewals.");
            throw;
        }
    }
}
