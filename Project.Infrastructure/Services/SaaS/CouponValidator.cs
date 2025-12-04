using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities.SaaS;
using Project.Domain.Shared.Enums;
using Project.Domain.Shared.Exceptions;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.Common;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Services;

namespace Project.Infrastructure.Services.SaaS;

public class CouponValidator : DomainService, ICouponValidator, ITransientDependency
{
    private readonly IRepository<SaasCoupon, Guid> _couponRepository;
    private readonly IClock _clock;

    public CouponValidator(IRepository<SaasCoupon, Guid> couponRepository, IClock clock)
    {
        _couponRepository = couponRepository;
        _clock = clock;
    }

    public async Task<bool> IsValidAsync(string code, Guid? tenantId, Guid? planId)
    {
        var coupon = await _couponRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.Code == code);
        if (coupon == null) return false;

        if (!coupon.IsActive) return false;
        if (_clock.Now < coupon.ValidFrom || _clock.Now > coupon.ValidTo) return false;
        if (coupon.MaxUses.HasValue && coupon.UsedCount >= coupon.MaxUses.Value) return false;

        // Check plan applicability
        // if (planId.HasValue && !string.IsNullOrEmpty(coupon.ApplicablePlans))
        // {
        //     // Parse JSON or list
        // }

        return true;
    }

    public async Task<decimal> CalculateDiscountAsync(string code, decimal amount)
    {
        var coupon = await _couponRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.Code == code);
        if (coupon == null) return 0;

        if (coupon.DiscountType == DiscountType.Percentage)
        {
            return amount * (coupon.DiscountPercentage ?? 0) / 100m;
        }
        else
        {
            return Math.Min(amount, coupon.DiscountAmount ?? 0);
        }
    }

    public async Task MarkCouponUsedAsync(string code, Guid tenantId)
    {
        var coupon = await _couponRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.Code == code);
        if (coupon != null)
        {
            coupon.UsedCount++;
            await _couponRepository.UpdateAsync(coupon);
        }
    }
}
