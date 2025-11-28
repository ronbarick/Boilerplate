using Project.Core.Entities.SaaS;

namespace Project.Core.Interfaces;

public interface IInvoiceManager
{
    Task<SaaSInvoice> GenerateInvoiceAsync(Guid paymentId);
    Task<SaaSInvoice> CreateInvoiceAsync(Guid subscriptionId, decimal amount, string currency, DateTime dueDate);
    Task MarkInvoiceAsPaidAsync(Guid invoiceId, DateTime paidDate);
    Task CancelInvoiceAsync(Guid invoiceId);
    Task<string> GenerateInvoicePdfAsync(Guid invoiceId);
    Task SendInvoiceEmailAsync(Guid invoiceId);
}
