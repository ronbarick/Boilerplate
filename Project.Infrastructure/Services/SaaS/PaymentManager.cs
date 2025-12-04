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

public class PaymentManager : DomainService, IPaymentManager, ITransientDependency
{
    private readonly IRepository<SaasTenantSubscriptionPayment, Guid> _paymentRepository;
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly IRazorpayService _razorpayService;
    private readonly IStripeService _stripeService;
    private readonly IInvoiceManager _invoiceManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock;

    public PaymentManager(
        IRepository<SaasTenantSubscriptionPayment, Guid> paymentRepository,
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        IRazorpayService razorpayService,
        IStripeService stripeService,
        IInvoiceManager invoiceManager,
        IGuidGenerator guidGenerator,
        IClock clock)
    {
        _paymentRepository = paymentRepository;
        _subscriptionRepository = subscriptionRepository;
        _razorpayService = razorpayService;
        _stripeService = stripeService;
        _invoiceManager = invoiceManager;
        _guidGenerator = guidGenerator;
        _clock = clock;
    }

    public async Task<SaasTenantSubscriptionPayment> ProcessPaymentAsync(Guid subscriptionId, decimal amount, string currency, PaymentGateway gateway, string paymentMethodId)
    {
        var subscription = await _subscriptionRepository.GetAsync(subscriptionId);
        if (subscription == null) throw new UserFriendlyException("Subscription not found");
        
        string gatewayOrderId = "";
        string? gatewayPaymentId = null;

        try
        {
            if (gateway == PaymentGateway.Razorpay)
            {
                gatewayOrderId = await _razorpayService.CreateOrderAsync(amount, currency, subscription.Id.ToString());
            }
            else if (gateway == PaymentGateway.Stripe)
            {
                gatewayOrderId = await _stripeService.CreatePaymentIntentAsync(amount, currency, paymentMethodId); 
            }
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"Payment initialization failed: {ex.Message}");
        }

        var payment = new SaasTenantSubscriptionPayment
        {
            Id = _guidGenerator.Create(),
            TenantId = subscription.TenantId,
            SubscriptionId = subscriptionId,
            Gateway = gateway,
            GatewayOrderId = gatewayOrderId,
            GatewayPaymentId = gatewayPaymentId,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Pending,
            PaymentDate = null
        };

        await _paymentRepository.InsertAsync(payment);
        return payment;
    }

    public async Task<bool> VerifyPaymentAsync(string paymentId, PaymentGateway gateway)
    {
        return true; 
    }

    public async Task HandlePaymentSuccessAsync(string gatewayPaymentId, PaymentGateway gateway)
    {
        var payment = await _paymentRepository.GetQueryable()
            .FirstOrDefaultAsync(p => p.GatewayPaymentId == gatewayPaymentId || p.GatewayOrderId == gatewayPaymentId);
        if (payment == null) return;

        if (payment.Status == PaymentStatus.Success) return;

        payment.Status = PaymentStatus.Success;
        payment.PaymentDate = _clock.Now;
        payment.GatewayPaymentId = gatewayPaymentId; 

        await _paymentRepository.UpdateAsync(payment);

        var subscription = await _subscriptionRepository.GetAsync(payment.SubscriptionId);
        if (subscription != null && (subscription.Status == SubscriptionStatus.Expired || subscription.Status == SubscriptionStatus.Trial))
        {
            subscription.Status = SubscriptionStatus.Active;
            await _subscriptionRepository.UpdateAsync(subscription);
        }

        await _invoiceManager.GenerateInvoiceAsync(payment.Id);
    }

    public async Task HandlePaymentFailureAsync(string gatewayPaymentId, PaymentGateway gateway, string reason)
    {
        var payment = await _paymentRepository.GetQueryable()
            .FirstOrDefaultAsync(p => p.GatewayPaymentId == gatewayPaymentId || p.GatewayOrderId == gatewayPaymentId);
        if (payment == null) return;

        payment.Status = PaymentStatus.Failed;
        payment.FailureReason = reason;
        
        await _paymentRepository.UpdateAsync(payment);
    }

    public async Task RefundPaymentAsync(Guid paymentId, decimal amount, string reason)
    {
        var payment = await _paymentRepository.GetAsync(paymentId);
        if (payment == null) throw new UserFriendlyException("Payment not found");
        
        if (payment.Gateway == PaymentGateway.Razorpay)
        {
            await _razorpayService.RefundPaymentAsync(payment.GatewayPaymentId!, amount);
        }
        else if (payment.Gateway == PaymentGateway.Stripe)
        {
            await _stripeService.RefundPaymentAsync(payment.GatewayPaymentId!, amount); 
        }

        payment.Status = PaymentStatus.Refunded; 
        
        await _paymentRepository.UpdateAsync(payment);
    }

    public async Task RetryFailedPaymentsAsync()
    {
        await Task.CompletedTask;
    }
}
