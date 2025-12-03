using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Domain.Entities.SaaS;
using Project.Domain.Interfaces;

namespace Project.Application.BackgroundJobs.SaaS;

public class UsageAlertWorker
{
    private readonly IRepository<SaaSFeatureUsage, Guid> _usageRepository;
    private readonly IRepository<SaaSFeature, Guid> _featureRepository;
    private readonly ILogger<UsageAlertWorker> _logger;

    public UsageAlertWorker(
        IRepository<SaaSFeatureUsage, Guid> usageRepository,
        IRepository<SaaSFeature, Guid> featureRepository,
        ILogger<UsageAlertWorker> logger)
    {
        _usageRepository = usageRepository;
        _featureRepository = featureRepository;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Checking for feature usage alerts...");
        
        try
        {
            var features = await _featureRepository.GetQueryable()
                .Where(x => x.AlertThresholdPercentage.HasValue)
                .ToListAsync();

            foreach (var feature in features)
            {
                var usages = await _usageRepository.GetQueryable()
                    .Where(x => x.FeatureName == feature.Name && !x.AlertSent)
                    .ToListAsync();

                foreach (var usage in usages)
                {
                    // Check if usage exceeds threshold
                    // This would require comparing against plan limits
                    _logger.LogInformation($"Checking usage for feature {feature.Name}, tenant {usage.TenantId}");
                    // TODO: Send alert if threshold exceeded
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking usage alerts.");
            throw;
        }
    }
}
