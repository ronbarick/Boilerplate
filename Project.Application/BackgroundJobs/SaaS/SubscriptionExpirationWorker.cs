using Microsoft.Extensions.Logging;
using Project.Domain.Interfaces;

namespace Project.Application.BackgroundJobs.SaaS;

public class SubscriptionExpirationWorker
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<SubscriptionExpirationWorker> _logger;

    public SubscriptionExpirationWorker(
        ISubscriptionManager subscriptionManager,
        ILogger<SubscriptionExpirationWorker> logger)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Starting subscription expiration check...");
        
        try
        {
            await _subscriptionManager.CheckExpirationsAsync();
            _logger.LogInformation("Subscription expiration check completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking subscription expirations.");
            throw;
        }
    }
}
