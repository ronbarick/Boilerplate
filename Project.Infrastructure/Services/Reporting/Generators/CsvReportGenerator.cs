using System.Globalization;
using CsvHelper;
using Project.Core.Dtos.Reporting;
using Project.Core.Interfaces.Reporting;

namespace Project.Infrastructure.Services.Reporting.Generators;

public class CsvReportGenerator : IReportGenerator
{
    public string Format => "csv";

    public async Task<ReportFileDto> GenerateAsync<T>(IEnumerable<T> data, string reportName, string? customSuffix = null)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(data);
        await writer.FlushAsync();

        var fileName = BuildFileName(reportName, customSuffix, "csv");

        return new ReportFileDto
        {
            FileName = fileName,
            Content = memoryStream.ToArray(),
            MimeType = "text/csv"
        };
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

        // Allow only alphanumeric, underscore, and dash. Max 50 chars.
        var sanitized = new string(suffix
            .Take(50)
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-')
            .ToArray());

        return sanitized;
    }
}
