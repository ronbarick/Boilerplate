namespace Project.Core.Dtos.Reporting;

public class ReportFileDto
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string MimeType { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; } // For stored files
}
