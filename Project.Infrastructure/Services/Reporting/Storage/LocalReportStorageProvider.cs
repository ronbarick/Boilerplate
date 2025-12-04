using Microsoft.AspNetCore.Hosting;
using Project.Domain.Dtos.Reporting;
using Project.Domain.Interfaces.Reporting;

namespace Project.Infrastructure.Services.Reporting.Storage;

public class LocalReportStorageProvider : IReportStorageProvider
{
    private readonly IWebHostEnvironment _environment;
    private const string ReportsFolder = "reports";

    public LocalReportStorageProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveReportAsync(ReportFileDto reportFile, Guid? tenantId)
    {
        var folderPath = Path.Combine(_environment.WebRootPath, ReportsFolder);
        
        if (tenantId.HasValue)
        {
            folderPath = Path.Combine(folderPath, tenantId.Value.ToString());
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, reportFile.FileName);
        await File.WriteAllBytesAsync(filePath, reportFile.Content);

        // Return relative URL
        var relativePath = $"/{ReportsFolder}";
        if (tenantId.HasValue)
        {
            relativePath += $"/{tenantId.Value}";
        }
        relativePath += $"/{reportFile.FileName}";

        return relativePath;
    }

    public async Task<ReportFileDto> GetReportAsync(string filePath)
    {
        // Security check to prevent path traversal
        var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
        if (!fullPath.StartsWith(_environment.WebRootPath))
        {
            throw new UnauthorizedAccessException("Invalid file path.");
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Report file not found.");
        }

        var content = await File.ReadAllBytesAsync(fullPath);
        var fileName = Path.GetFileName(fullPath);
        var extension = Path.GetExtension(fullPath).ToLower();
        
        var mimeType = extension switch
        {
            ".csv" => "text/csv",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };

        return new ReportFileDto
        {
            FileName = fileName,
            Content = content,
            MimeType = mimeType,
            DownloadUrl = filePath
        };
    }
}
