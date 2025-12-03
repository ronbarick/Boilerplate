using System;
using System.Threading.Tasks;
using Project.Application.SaaS.Dtos;

using Project.Domain.Interfaces;

namespace Project.Application.SaaS.Payments;

public interface IPaymentAppService : IApplicationService
{
    Task<SaasTenantSubscriptionPaymentDto> ProcessPaymentAsync(Guid subscriptionId, decimal amount, string currency, PaymentGateway gateway, string paymentMethodId);
    Task HandlePaymentSuccessAsync(string gatewayPaymentId, PaymentGateway gateway);
    Task HandlePaymentFailureAsync(string gatewayPaymentId, PaymentGateway gateway, string reason);
}
