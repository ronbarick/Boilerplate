using Project.Domain.Entities.SaaS;

namespace Project.Domain.Interfaces;

public interface IInvoicePdfGenerator
{
    Task<byte[]> GenerateInvoicePdfAsync(SaaSInvoice invoice, string templatePath);
    Task<string> SaveInvoicePdfAsync(SaaSInvoice invoice, byte[] pdfBytes);
}
