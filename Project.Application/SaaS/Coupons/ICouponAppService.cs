using System;
using System.Threading.Tasks;
using Project.Core.Interfaces;

namespace Project.Application.SaaS.Coupons;

public interface ICouponAppService : IApplicationService
{
    Task<bool> ValidateCouponAsync(string code, Guid? tenantId, Guid? planId);
    Task<decimal> CalculateDiscountAsync(string code, decimal amount);
    Task ApplyCouponAsync(string code, Guid tenantId);
}
