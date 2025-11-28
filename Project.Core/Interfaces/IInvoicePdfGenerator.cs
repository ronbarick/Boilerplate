using Project.Core.Entities.SaaS;

namespace Project.Core.Interfaces;

public interface IInvoicePdfGenerator
{
    Task<byte[]> GenerateInvoicePdfAsync(SaaSInvoice invoice, string templatePath);
    Task<string> SaveInvoicePdfAsync(SaaSInvoice invoice, byte[] pdfBytes);
}
