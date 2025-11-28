namespace Project.Application.Tenants.Dtos;

public class IsTenantAvailableOutput
{
    public TenantAvailabilityState State { get; set; }
    public Guid? TenantId { get; set; }
    public string? Name { get; set; }
}

public enum TenantAvailabilityState
{
    Available = 1,
    InActive = 2,
    NotFound = 3
}
