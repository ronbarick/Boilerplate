using System;
using System.Threading.Tasks;
using Project.Application.Services;
using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.SaaS.Coupons;

public class CouponAppService : AppServiceBase, ICouponAppService
{
    private readonly ICouponValidator _couponValidator;

    public CouponAppService(
        ICouponValidator couponValidator,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _couponValidator = couponValidator;
    }

    public async Task<bool> ValidateCouponAsync(string code, Guid? tenantId, Guid? planId)
    {
        return await _couponValidator.IsValidAsync(code, tenantId, planId);
    }

    public async Task<decimal> CalculateDiscountAsync(string code, decimal amount)
    {
        return await _couponValidator.CalculateDiscountAsync(code, amount);
    }

    public async Task ApplyCouponAsync(string code, Guid tenantId)
    {
        await _couponValidator.MarkCouponUsedAsync(code, tenantId);
    }
}
