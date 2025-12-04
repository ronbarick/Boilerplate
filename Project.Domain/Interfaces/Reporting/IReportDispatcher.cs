using Project.Domain.Dtos.Reporting;

namespace Project.Domain.Interfaces.Reporting;

public interface IReportDispatcher
{
    Task<ReportFileDto> DispatchAsync<T>(string format, IEnumerable<T> data, string reportName, string? customSuffix = null);
}
