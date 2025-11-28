using System;
using System.Threading.Tasks;
using Project.Core.Interfaces;
using Project.Core.Interfaces.DependencyInjection;
using Project.Core.Services;

namespace Project.Infrastructure.Services.SaaS;

public class RazorpayService : DomainService, IRazorpayService, ITransientDependency
{
    public async Task<string> CreateOrderAsync(decimal amount, string currency, string receipt)
    {
        // Mock implementation
        // In real app: use RazorpayClient to create order
        await Task.Delay(100);
        return $"order_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }

    public async Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature)
    {
        // Mock verification
        return true;
    }

    public async Task<string> CreateSubscriptionAsync(string planId, int totalCount, int? startAt = null)
    {
        await Task.Delay(100);
        return $"sub_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }

    public async Task CancelSubscriptionAsync(string subscriptionId)
    {
        await Task.Delay(100);
    }

    public async Task<string> CreateCustomerAsync(string name, string email, string contact)
    {
        await Task.Delay(100);
        return $"cust_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
    }

    public async Task RefundPaymentAsync(string paymentId, decimal amount)
    {
        await Task.Delay(100);
    }
}
