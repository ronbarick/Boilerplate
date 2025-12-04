using Project.Domain.Dtos.Reporting;

namespace Project.Domain.Interfaces.Reporting;

public interface IReportService
{
    Task<ReportFileDto> GenerateReportAsync<T>(ReportRequestDto input, Func<Task<List<T>>> dataRetriever, string reportName);
}
