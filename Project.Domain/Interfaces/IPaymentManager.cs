using Project.Domain.Entities.SaaS;


namespace Project.Domain.Interfaces;

public interface IPaymentManager
{
    Task<SaasTenantSubscriptionPayment> ProcessPaymentAsync(Guid subscriptionId, decimal amount, string currency, PaymentGateway gateway, string paymentMethodId);
    Task<bool> VerifyPaymentAsync(string paymentId, PaymentGateway gateway);
    Task HandlePaymentSuccessAsync(string gatewayPaymentId, PaymentGateway gateway);
    Task HandlePaymentFailureAsync(string gatewayPaymentId, PaymentGateway gateway, string reason);
    Task RefundPaymentAsync(Guid paymentId, decimal amount, string reason);
    Task RetryFailedPaymentsAsync();
}
