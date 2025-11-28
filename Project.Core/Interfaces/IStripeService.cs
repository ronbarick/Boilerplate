using Project.Core.Entities.SaaS;

namespace Project.Core.Interfaces;

public interface IStripeService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency, string customerId);
    Task<string> CreateCustomerAsync(string email, string name);
    Task<string> CreateSubscriptionAsync(string customerId, string priceId);
    Task CancelSubscriptionAsync(string subscriptionId);
    Task RefundPaymentAsync(string paymentIntentId, decimal amount);
    Task<bool> VerifyWebhookSignatureAsync(string json, string signature);
}
