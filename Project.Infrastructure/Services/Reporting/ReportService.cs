using Hangfire;
using Microsoft.Extensions.Logging;
using Project.Core.Dtos.Reporting;
using Project.Core.Interfaces.Reporting;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Services.Reporting;

public class ReportService : IReportService
{
    private readonly IReportDispatcher _dispatcher;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IReportStorageProvider _storageProvider;
    private readonly ILogger<ReportService> _logger;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUser _currentUser;

    // Configurable threshold
    private const int BackgroundJobThreshold = 1000;

    public ReportService(
        IReportDispatcher dispatcher,
        IBackgroundJobClient backgroundJobClient,
        IReportStorageProvider storageProvider,
        ILogger<ReportService> logger,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser)
    {
        _dispatcher = dispatcher;
        _backgroundJobClient = backgroundJobClient;
        _storageProvider = storageProvider;
        _logger = logger;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
    }

    public async Task<ReportFileDto> GenerateReportAsync<T>(ReportRequestDto input, Func<Task<List<T>>> dataRetriever, string reportName)
    {
        _logger.LogInformation("Generating report '{ReportName}' (Format: {Format}) for User {UserId}, Tenant {TenantId}", 
            reportName, input.ReportType, _currentUser.Id, _currentTenant.Id);

        var data = await dataRetriever();

        // Check threshold for background processing
        if (input.ForceBackgroundJob || data.Count > BackgroundJobThreshold)
        {
            _logger.LogInformation("Data count ({Count}) exceeds threshold. Saving to storage.", data.Count);
            
            var reportFile = await _dispatcher.DispatchAsync(input.ReportType, data, reportName, input.CustomSuffix);
            var url = await _storageProvider.SaveReportAsync(reportFile, _currentTenant.Id);
             
            _logger.LogInformation("Report '{FileName}' saved to storage. Size: {Size} bytes", 
                reportFile.FileName, reportFile.Content.Length);

            return new ReportFileDto
            {
                FileName = reportFile.FileName,
                MimeType = reportFile.MimeType,
                DownloadUrl = url,
                Content = Array.Empty<byte>() // Don't return content for large files
            };
        }

        var result = await _dispatcher.DispatchAsync(input.ReportType, data, reportName, input.CustomSuffix);
        
        _logger.LogInformation("Report '{FileName}' generated successfully. Size: {Size} bytes", 
            result.FileName, result.Content.Length);

        return result;
    }
}
