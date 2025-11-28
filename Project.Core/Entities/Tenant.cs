using System;
using Project.Core.Entities.Base;

namespace Project.Core.Entities;

public class Tenant : FullAuditedEntity
{
    public string Name { get; set; } = null!;
    public string TenancyName { get; set; } = null!;
    public string? ConnectionString { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public long? EditionId { get; set; }
    public string? EditionName { get; set; } // SaaS plan name
    public bool IsInTrialPeriod { get; set; } = false;
    public DateTime? SubscriptionEndDate { get; set; }
}
