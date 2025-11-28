using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Core.Entities.SaaS;
using Project.Core.Enums;
using Project.Core.Interfaces.Common;
using Project.Infrastructure.Data;

namespace Project.Migration.Seeding;

public class SaaSDataSeeder
{
    private readonly AppDbContext _context;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<SaaSDataSeeder> _logger;

    public SaaSDataSeeder(
        AppDbContext context,
        IGuidGenerator guidGenerator,
        ILogger<SaaSDataSeeder> logger)
    {
        _context = context;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting SaaS data seeding...");

        await SeedFeaturesAsync();
        await SeedPlansAsync();
        await SeedPlanFeaturesAsync();

        await _context.SaveChangesAsync();
        _logger.LogInformation("SaaS data seeding completed.");
    }

    private async Task SeedFeaturesAsync()
    {
        if (await _context.SaaSFeatures.AnyAsync())
        {
            _logger.LogInformation("Features already seeded, skipping...");
            return;
        }

        var features = new List<SaaSFeature>
        {
            new SaaSFeature
            {
                Id = _guidGenerator.Create(),
                Name = "MaxProjects",
                DisplayName = "Maximum Projects",
                Description = "Maximum number of projects allowed",
                DefaultValue = "5",
                FeatureType = FeatureType.Numeric,
                GroupName = "Projects",
                AlertThresholdPercentage = 80
            },
            new SaaSFeature
            {
                Id = _guidGenerator.Create(),
                Name = "MaxUsers",
                DisplayName = "Maximum Users",
                Description = "Maximum number of users allowed",
                DefaultValue = "10",
                FeatureType = FeatureType.Numeric,
                GroupName = "Users",
                AlertThresholdPercentage = 90
            },
            new SaaSFeature
            {
                Id = _guidGenerator.Create(),
                Name = "AdvancedReporting",
                DisplayName = "Advanced Reporting",
                Description = "Access to advanced reporting features",
                DefaultValue = "false",
                FeatureType = FeatureType.Boolean,
                GroupName = "Reporting"
            },
            new SaaSFeature
            {
                Id = _guidGenerator.Create(),
                Name = "APIAccess",
                DisplayName = "API Access",
                Description = "Access to REST API",
                DefaultValue = "false",
                FeatureType = FeatureType.Boolean,
                GroupName = "Integration"
            },
            new SaaSFeature
            {
                Id = _guidGenerator.Create(),
                Name = "PrioritySupport",
                DisplayName = "Priority Support",
                Description = "24/7 priority customer support",
                DefaultValue = "false",
                FeatureType = FeatureType.Boolean,
                GroupName = "Support"
            },
            new SaaSFeature
            {
                Id = _guidGenerator.Create(),
                Name = "StorageGB",
                DisplayName = "Storage (GB)",
                Description = "Storage space in gigabytes",
                DefaultValue = "10",
                FeatureType = FeatureType.Numeric,
                GroupName = "Storage",
                AlertThresholdPercentage = 85
            }
        };

        await _context.SaaSFeatures.AddRangeAsync(features);
        _logger.LogInformation($"Seeded {features.Count} features.");
    }

    private async Task SeedPlansAsync()
    {
        if (await _context.SaaSPlans.AnyAsync())
        {
            _logger.LogInformation("Plans already seeded, skipping...");
            return;
        }

        var plans = new List<SaasPlan>
        {
            new SaasPlan
            {
                Id = _guidGenerator.Create(),
                Name = "Free",
                DisplayName = "Free Plan",
                Description = "Perfect for trying out our platform",
                BillingCycle = BillingCycle.Monthly,
                Price = 0,
                Currency = "USD",
                IsFree = true,
                TrialDays = 0,
                IsActive = true,
                Color = "#9E9E9E",
                SortOrder = 1
            },
            new SaasPlan
            {
                Id = _guidGenerator.Create(),
                Name = "Starter",
                DisplayName = "Starter Plan",
                Description = "Great for small teams getting started",
                BillingCycle = BillingCycle.Monthly,
                Price = 29,
                Currency = "USD",
                IsFree = false,
                TrialDays = 14,
                IsActive = true,
                Color = "#2196F3",
                SortOrder = 2
            },
            new SaasPlan
            {
                Id = _guidGenerator.Create(),
                Name = "Professional",
                DisplayName = "Professional Plan",
                Description = "For growing businesses",
                BillingCycle = BillingCycle.Monthly,
                Price = 79,
                Currency = "USD",
                IsFree = false,
                TrialDays = 14,
                IsActive = true,
                Color = "#4CAF50",
                SortOrder = 3
            },
            new SaasPlan
            {
                Id = _guidGenerator.Create(),
                Name = "Enterprise",
                DisplayName = "Enterprise Plan",
                Description = "For large organizations with advanced needs",
                BillingCycle = BillingCycle.Monthly,
                Price = 199,
                Currency = "USD",
                IsFree = false,
                TrialDays = 30,
                IsActive = true,
                Color = "#9C27B0",
                SortOrder = 4
            }
        };

        await _context.SaaSPlans.AddRangeAsync(plans);
        _logger.LogInformation($"Seeded {plans.Count} plans.");
    }

    private async Task SeedPlanFeaturesAsync()
    {
        if (await _context.SaasPlanFeatures.AnyAsync())
        {
            _logger.LogInformation("Plan features already seeded, skipping...");
            return;
        }

        var plans = await _context.SaaSPlans.ToListAsync();
        var freePlan = plans.First(p => p.Name == "Free");
        var starterPlan = plans.First(p => p.Name == "Starter");
        var professionalPlan = plans.First(p => p.Name == "Professional");
        var enterprisePlan = plans.First(p => p.Name == "Enterprise");

        var planFeatures = new List<SaasPlanFeature>
        {
            // Free Plan
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = freePlan.Id, FeatureName = "MaxProjects", FeatureValue = "3", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = freePlan.Id, FeatureName = "MaxUsers", FeatureValue = "5", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = freePlan.Id, FeatureName = "StorageGB", FeatureValue = "5", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = freePlan.Id, FeatureName = "AdvancedReporting", FeatureValue = "false", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = freePlan.Id, FeatureName = "APIAccess", FeatureValue = "false", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = freePlan.Id, FeatureName = "PrioritySupport", FeatureValue = "false", FeatureType = FeatureType.Boolean },

            // Starter Plan
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = starterPlan.Id, FeatureName = "MaxProjects", FeatureValue = "10", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = starterPlan.Id, FeatureName = "MaxUsers", FeatureValue = "15", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = starterPlan.Id, FeatureName = "StorageGB", FeatureValue = "50", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = starterPlan.Id, FeatureName = "AdvancedReporting", FeatureValue = "false", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = starterPlan.Id, FeatureName = "APIAccess", FeatureValue = "true", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = starterPlan.Id, FeatureName = "PrioritySupport", FeatureValue = "false", FeatureType = FeatureType.Boolean },

            // Professional Plan
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = professionalPlan.Id, FeatureName = "MaxProjects", FeatureValue = "50", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = professionalPlan.Id, FeatureName = "MaxUsers", FeatureValue = "50", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = professionalPlan.Id, FeatureName = "StorageGB", FeatureValue = "200", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = professionalPlan.Id, FeatureName = "AdvancedReporting", FeatureValue = "true", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = professionalPlan.Id, FeatureName = "APIAccess", FeatureValue = "true", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = professionalPlan.Id, FeatureName = "PrioritySupport", FeatureValue = "false", FeatureType = FeatureType.Boolean },

            // Enterprise Plan
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = enterprisePlan.Id, FeatureName = "MaxProjects", FeatureValue = "999999", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = enterprisePlan.Id, FeatureName = "MaxUsers", FeatureValue = "999999", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = enterprisePlan.Id, FeatureName = "StorageGB", FeatureValue = "1000", FeatureType = FeatureType.Numeric },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = enterprisePlan.Id, FeatureName = "AdvancedReporting", FeatureValue = "true", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = enterprisePlan.Id, FeatureName = "APIAccess", FeatureValue = "true", FeatureType = FeatureType.Boolean },
            new SaasPlanFeature { Id = _guidGenerator.Create(), PlanId = enterprisePlan.Id, FeatureName = "PrioritySupport", FeatureValue = "true", FeatureType = FeatureType.Boolean }
        };

        await _context.SaasPlanFeatures.AddRangeAsync(planFeatures);
        _logger.LogInformation($"Seeded {planFeatures.Count} plan features.");
    }
}
