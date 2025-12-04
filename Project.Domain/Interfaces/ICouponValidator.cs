using Project.Domain.Entities.SaaS;

namespace Project.Domain.Interfaces;

public interface ICouponValidator
{
    Task<bool> IsValidAsync(string code, Guid? tenantId, Guid? planId);
    Task<decimal> CalculateDiscountAsync(string code, decimal amount);
    Task MarkCouponUsedAsync(string code, Guid tenantId);
}
