using System;
using System.Threading.Tasks;
using AutoMapper;
using Project.Application.SaaS.Dtos;
using Project.Application.Services;

using Project.Domain.Interfaces;
using Project.Domain.Localization;

namespace Project.Application.SaaS.Payments;

public class PaymentAppService : AppServiceBase, IPaymentAppService
{
    private readonly IPaymentManager _paymentManager;
    private readonly IMapper _mapper;

    public PaymentAppService(
        IPaymentManager paymentManager,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _paymentManager = paymentManager;
        _mapper = mapper;
    }

    public async Task<SaasTenantSubscriptionPaymentDto> ProcessPaymentAsync(Guid subscriptionId, decimal amount, string currency, PaymentGateway gateway, string paymentMethodId)
    {
        var payment = await _paymentManager.ProcessPaymentAsync(subscriptionId, amount, currency, gateway, paymentMethodId);
        return _mapper.Map<SaasTenantSubscriptionPaymentDto>(payment);
    }

    public async Task HandlePaymentSuccessAsync(string gatewayPaymentId, PaymentGateway gateway)
    {
        await _paymentManager.HandlePaymentSuccessAsync(gatewayPaymentId, gateway);
    }

    public async Task HandlePaymentFailureAsync(string gatewayPaymentId, PaymentGateway gateway, string reason)
    {
        await _paymentManager.HandlePaymentFailureAsync(gatewayPaymentId, gateway, reason);
    }
}
