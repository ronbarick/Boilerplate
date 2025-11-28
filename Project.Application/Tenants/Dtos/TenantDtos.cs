using System;

namespace Project.Application.Tenants.Dtos;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string TenancyName { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsInTrialPeriod { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}

public class CreateTenantDto
{
    public string Name { get; set; } = null!;
    public string TenancyName { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
    public long? EditionId { get; set; }
}
