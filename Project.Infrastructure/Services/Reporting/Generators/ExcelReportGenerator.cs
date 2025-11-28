using ClosedXML.Excel;
using Project.Core.Dtos.Reporting;
using Project.Core.Interfaces.Reporting;

namespace Project.Infrastructure.Services.Reporting.Generators;

public class ExcelReportGenerator : IReportGenerator
{
    public string Format => "excel";

    public Task<ReportFileDto> GenerateAsync<T>(IEnumerable<T> data, string reportName, string? customSuffix = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(reportName);
        
        // Add data
        worksheet.Cell(1, 1).InsertTable(data);
        
        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        var fileName = BuildFileName(reportName, customSuffix, "xlsx");

        return Task.FromResult(new ReportFileDto
        {
            FileName = fileName,
            Content = memoryStream.ToArray(),
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
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
}
