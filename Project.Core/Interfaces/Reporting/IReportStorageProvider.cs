using Project.Core.Dtos.Reporting;

namespace Project.Core.Interfaces.Reporting;

public interface IReportStorageProvider
{
    Task<string> SaveReportAsync(ReportFileDto reportFile, Guid? tenantId);
    Task<ReportFileDto> GetReportAsync(string filePath);
}
