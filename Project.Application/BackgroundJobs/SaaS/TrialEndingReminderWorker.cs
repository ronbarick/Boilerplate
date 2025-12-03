using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Domain.Entities.SaaS;

using Project.Domain.Interfaces;

namespace Project.Application.BackgroundJobs.SaaS;

public class TrialEndingReminderWorker
{
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly ILogger<TrialEndingReminderWorker> _logger;

    public TrialEndingReminderWorker(
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        ILogger<TrialEndingReminderWorker> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Checking for trials ending soon...");
        
        try
        {
            var threeDaysFromNow = DateTime.UtcNow.AddDays(3);
            
            var endingTrials = await _subscriptionRepository.GetQueryable()
                .Where(x => x.Status == SubscriptionStatus.Trial 
                    && x.TrialEndDate.HasValue 
                    && x.TrialEndDate.Value.Date == threeDaysFromNow.Date)
                .ToListAsync();

            _logger.LogInformation($"Found {endingTrials.Count} trials ending in 3 days.");
            
            // TODO: Send email notifications for each ending trial
            foreach (var subscription in endingTrials)
            {
                _logger.LogInformation($"Trial ending for tenant {subscription.TenantId}");
                // Email sending would go here
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking trial endings.");
            throw;
        }
    }
}
