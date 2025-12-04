using Project.Domain.Dtos.Reporting;

namespace Project.Domain.Interfaces.Reporting;

public interface IReportStorageProvider
{
    Task<string> SaveReportAsync(ReportFileDto reportFile, Guid? tenantId);
    Task<ReportFileDto> GetReportAsync(string filePath);
}
