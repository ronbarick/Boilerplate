using Project.Core.Dtos.Reporting;

namespace Project.Core.Interfaces.Reporting;

public interface IReportGenerator
{
    string Format { get; }
    Task<ReportFileDto> GenerateAsync<T>(IEnumerable<T> data, string reportName, string? customSuffix = null);
}
