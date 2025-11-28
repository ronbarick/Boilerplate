using Project.Core.Interfaces;
using Project.Core.Interfaces.DependencyInjection;
using Project.Core.Services;

namespace Project.Infrastructure.Services.SaaS;

public class StripeService : DomainService, IStripeService, ITransientDependency
{
    public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency, string customerId)
    {
        // Mock implementation
        await Task.Delay(100);
        return $"pi_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }

    public async Task<string> CreateCustomerAsync(string email, string name)
    {
        await Task.Delay(100);
        return $"cus_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }

    public async Task<string> CreateSubscriptionAsync(string customerId, string priceId)
    {
        await Task.Delay(100);
        return $"sub_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }

    public async Task CancelSubscriptionAsync(string subscriptionId)
    {
        await Task.Delay(100);
    }

    public async Task RefundPaymentAsync(string paymentIntentId, decimal amount)
    {
        await Task.Delay(100);
    }

    public async Task<bool> VerifyWebhookSignatureAsync(string json, string signature)
    {
        return true;
    }
}
