using Project.Domain.Dtos.Reporting;
using Project.Domain.Interfaces.Reporting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Project.Infrastructure.Services.Reporting.Generators;

public class PdfReportGenerator : IReportGenerator
{
    public string Format => "pdf";

    public Task<ReportFileDto> GenerateAsync<T>(IEnumerable<T> data, string reportName, string? customSuffix = null)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text(reportName)
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        // Define columns based on properties of T
                        var properties = typeof(T).GetProperties();
                        
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in properties)
                            {
                                columns.RelativeColumn();
                            }
                        });

                        // Header
                        table.Header(header =>
                        {
                            foreach (var prop in properties)
                            {
                                header.Cell().Element(CellStyle).Text(prop.Name);
                            }
                        });

                        // Data
                        foreach (var item in data)
                        {
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(item)?.ToString() ?? "";
                                table.Cell().Element(CellStyle).Text(value);
                            }
                        }
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

        var content = document.GeneratePdf();
        var fileName = BuildFileName(reportName, customSuffix, "pdf");

        return Task.FromResult(new ReportFileDto
        {
            FileName = fileName,
            Content = content,
            MimeType = "application/pdf"
        });
    }

    private static string BuildFileName(string reportName, string? customSuffix, string extension)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var sanitizedSuffix = SanitizeSuffix(customSuffix);
        
        return string.IsNullOrEmpty(sanitizedSuffix)
            ? $"{reportName}_{timestamp}.{extension}"
            : $"{reportName}_{sanitizedSuffix}_{timestamp}.{extension}";
    }

    private static string SanitizeSuffix(string? suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return string.Empty;

        var sanitized = new string(suffix
            .Take(50)
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-')
            .ToArray());

        return sanitized;
    }

    static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}
