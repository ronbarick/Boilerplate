namespace Project.Core.Emailing;

/// <summary>
/// Represents an email attachment.
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// File name of the attachment.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Content of the attachment as byte array.
    /// </summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// MIME content type (e.g., "application/pdf", "image/png").
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";
}
