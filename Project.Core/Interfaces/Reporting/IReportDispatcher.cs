using Project.Core.Dtos.Reporting;

namespace Project.Core.Interfaces.Reporting;

public interface IReportDispatcher
{
    Task<ReportFileDto> DispatchAsync<T>(string format, IEnumerable<T> data, string reportName, string? customSuffix = null);
}
