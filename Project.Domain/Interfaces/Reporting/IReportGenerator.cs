using Project.Domain.Dtos.Reporting;

namespace Project.Domain.Interfaces.Reporting;

public interface IReportGenerator
{
    string Format { get; }
    Task<ReportFileDto> GenerateAsync<T>(IEnumerable<T> data, string reportName, string? customSuffix = null);
}
