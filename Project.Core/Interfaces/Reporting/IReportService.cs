using Project.Core.Dtos.Reporting;

namespace Project.Core.Interfaces.Reporting;

public interface IReportService
{
    Task<ReportFileDto> GenerateReportAsync<T>(ReportRequestDto input, Func<Task<List<T>>> dataRetriever, string reportName);
}
