using System;
using System.Threading.Tasks;
using Project.Domain.Entities.SaaS;
using Project.Domain.Shared.Enums;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.Common;
using Project.Domain.Interfaces.DependencyInjection;
using Project.Domain.Services;

namespace Project.Infrastructure.Services.SaaS;

public class InvoiceManager : DomainService, IInvoiceManager, ITransientDependency
{
    private readonly IRepository<SaaSInvoice, Guid> _invoiceRepository;
    private readonly IRepository<SaasTenantSubscriptionPayment, Guid> _paymentRepository;
    private readonly IRepository<SaasTenantSubscription, Guid> _subscriptionRepository;
    private readonly IInvoicePdfGenerator _pdfGenerator;
    private readonly ITaxCalculator _taxCalculator;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock;

    public InvoiceManager(
        IRepository<SaaSInvoice, Guid> invoiceRepository,
        IRepository<SaasTenantSubscriptionPayment, Guid> paymentRepository,
        IRepository<SaasTenantSubscription, Guid> subscriptionRepository,
        IInvoicePdfGenerator pdfGenerator,
        ITaxCalculator taxCalculator,
        IGuidGenerator guidGenerator,
        IClock clock)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _subscriptionRepository = subscriptionRepository;
        _pdfGenerator = pdfGenerator;
        _taxCalculator = taxCalculator;
        _guidGenerator = guidGenerator;
        _clock = clock;
    }

    public async Task<SaaSInvoice> GenerateInvoiceAsync(Guid paymentId)
    {
        var payment = await _paymentRepository.GetAsync(paymentId);
        if (payment == null) throw new Exception("Payment not found"); // Or custom exception

        var subscription = await _subscriptionRepository.GetAsync(payment.SubscriptionId);
        if (subscription == null) throw new Exception("Subscription not found");

        // Calculate Tax
        string country = "IN"; 
        string state = ""; 
        decimal taxAmount = _taxCalculator.CalculateTax(payment.Amount, country, state, subscription.TaxId);

        var invoice = new SaaSInvoice
        {
            Id = _guidGenerator.Create(),
            TenantId = payment.TenantId ?? Guid.Empty, 
            SubscriptionId = payment.SubscriptionId,
            PaymentId = paymentId,
            InvoiceNumber = GenerateInvoiceNumber(),
            Amount = payment.Amount,
            Tax = taxAmount,
            TotalAmount = payment.Amount + taxAmount,
            Currency = payment.Currency,
            Status = InvoiceStatus.Paid, 
            IssuedDate = _clock.Now,
            DueDate = _clock.Now, 
            PaidDate = _clock.Now
        };

        await _invoiceRepository.InsertAsync(invoice);
        
        await SendInvoiceEmailAsync(invoice.Id);

        return invoice;
    }

    public async Task<SaaSInvoice> CreateInvoiceAsync(Guid subscriptionId, decimal amount, string currency, DateTime dueDate)
    {
        var subscription = await _subscriptionRepository.GetAsync(subscriptionId);
        if (subscription == null) throw new Exception("Subscription not found");
        
        var invoice = new SaaSInvoice
        {
            Id = _guidGenerator.Create(),
            TenantId = subscription.TenantId,
            SubscriptionId = subscriptionId,
            InvoiceNumber = GenerateInvoiceNumber(),
            Amount = amount,
            Tax = 0, 
            TotalAmount = amount,
            Currency = currency,
            Status = InvoiceStatus.Draft,
            IssuedDate = _clock.Now,
            DueDate = dueDate
        };

        await _invoiceRepository.InsertAsync(invoice);
        return invoice;
    }

    public async Task MarkInvoiceAsPaidAsync(Guid invoiceId, DateTime paidDate)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);
        if (invoice == null) return;

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidDate = paidDate;
        await _invoiceRepository.UpdateAsync(invoice);
    }

    public async Task CancelInvoiceAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);
        if (invoice == null) return;

        invoice.Status = InvoiceStatus.Cancelled;
        await _invoiceRepository.UpdateAsync(invoice);
    }

    public async Task<string> GenerateInvoicePdfAsync(Guid invoiceId)
    {
        return "path/to/pdf";
    }

    public async Task SendInvoiceEmailAsync(Guid invoiceId)
    {
        await Task.CompletedTask;
    }

    private string GenerateInvoiceNumber()
    {
        return $"INV-{_clock.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
    }
}
