using Project.Domain.Entities.SaaS;

namespace Project.Domain.Interfaces;

public interface IRazorpayService
{
    Task<string> CreateOrderAsync(decimal amount, string currency, string receipt);
    Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature);
    Task<string> CreateSubscriptionAsync(string planId, int totalCount, int? startAt = null);
    Task CancelSubscriptionAsync(string subscriptionId);
    Task<string> CreateCustomerAsync(string name, string email, string contact);
    Task RefundPaymentAsync(string paymentId, decimal amount);
}
