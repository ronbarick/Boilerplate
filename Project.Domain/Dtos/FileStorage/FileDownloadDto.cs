namespace Project.Domain.Dtos.FileStorage;

/// <summary>
/// DTO for file download containing stream and metadata
/// </summary>
public class FileDownloadDto
{
    public Stream Stream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
}
