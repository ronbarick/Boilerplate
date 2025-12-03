using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;
using Project.Domain.Entities.SaaS;

using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.SaaS.Analytics;

public class AnalyticsAppService : AppServiceBase, IAnalyticsAppService
{
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly IRepository<SaasTenantSubscriptionPayment, Guid> _paymentRepository;

    public AnalyticsAppService(
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        IRepository<SaasTenantSubscriptionPayment, Guid> paymentRepository,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<SaaSAnalyticsDto> GetAnalyticsAsync(Guid? tenantId = null)
    {
        var subscriptionsQuery = _subscriptionRepository.GetQueryable();
        var paymentsQuery = _paymentRepository.GetQueryable();

        if (tenantId.HasValue)
        {
            subscriptionsQuery = subscriptionsQuery.Where(x => x.TenantId == tenantId.Value);
            paymentsQuery = paymentsQuery.Where(x => x.TenantId == tenantId.Value);
        }

        var totalSubscriptions = await subscriptionsQuery.CountAsync();
        var activeSubscriptions = await subscriptionsQuery.CountAsync(x => x.Status == SubscriptionStatus.Active);
        var trialSubscriptions = await subscriptionsQuery.CountAsync(x => x.Status == SubscriptionStatus.Trial);
        var totalRevenue = await paymentsQuery.Where(x => x.Status == PaymentStatus.Success).SumAsync(x => x.Amount);
        var monthlyRevenue = await paymentsQuery
            .Where(x => x.Status == PaymentStatus.Success && x.PaymentDate.HasValue && x.PaymentDate.Value.Month == DateTime.UtcNow.Month)
            .SumAsync(x => x.Amount);

        return new SaaSAnalyticsDto
        {
            TotalSubscriptions = totalSubscriptions,
            ActiveSubscriptions = activeSubscriptions,
            TrialSubscriptions = trialSubscriptions,
            TotalRevenue = totalRevenue,
            MonthlyRevenue = monthlyRevenue
        };
    }
}
