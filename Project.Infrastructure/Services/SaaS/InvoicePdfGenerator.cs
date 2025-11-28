using Project.Core.Interfaces;
using Project.Core.Interfaces.DependencyInjection;
using Project.Core.Services;
using Project.Core.Entities.SaaS;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Project.Infrastructure.Services.SaaS;

public class InvoicePdfGenerator : DomainService, IInvoicePdfGenerator, ITransientDependency
{
    public InvoicePdfGenerator()
    {
        // License configuration (Community license for open source/free)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(SaaSInvoice invoice, string templatePath)
    {
        // In a real app, we might use the templatePath to load HTML or specific layout config
        // Here we use QuestPDF fluent API to build the document programmatically
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"Invoice #{invoice.InvoiceNumber}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Text($"Date: {invoice.IssuedDate:d}");
                        x.Item().Text($"Due Date: {invoice.DueDate:d}");
                        x.Item().Text($"Status: {invoice.Status}");

                        x.Item().LineHorizontal(1);

                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Description");
                                header.Cell().Element(CellStyle).AlignRight().Text("Amount");
                                header.Cell().Element(CellStyle).AlignRight().Text("Currency");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                                }
                            });

                            // Item
                            table.Cell().Element(CellStyle).Text("Subscription Charge");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{invoice.Amount:F2}");
                            table.Cell().Element(CellStyle).AlignRight().Text(invoice.Currency);

                            // Tax
                            table.Cell().Element(CellStyle).Text("Tax");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{invoice.Tax:F2}");
                            table.Cell().Element(CellStyle).AlignRight().Text(invoice.Currency);

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                            }
                        });

                        x.Item().AlignRight().Text($"Total: {invoice.TotalAmount:F2} {invoice.Currency}").FontSize(14).SemiBold();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        });

        return await Task.FromResult(document.GeneratePdf());
    }

    public async Task<string> SaveInvoicePdfAsync(SaaSInvoice invoice, byte[] pdfBytes)
    {
        // Save to file system or blob storage
        // For now, just return a fake path
        var fileName = $"invoice_{invoice.InvoiceNumber}.pdf";
        // In real app: _blobContainer.SaveAsync(fileName, pdfBytes);
        return $"/invoices/{fileName}";
    }
}
