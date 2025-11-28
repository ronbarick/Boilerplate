namespace Project.Core.Emailing;

/// <summary>
/// Represents an email message with full support for attachments, CC, BCC.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Recipient email address.
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Email subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Email body content.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Whether the body is HTML.
    /// </summary>
    public bool IsBodyHtml { get; set; } = true;

    /// <summary>
    /// List of attachments.
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Carbon copy recipients (comma-separated).
    /// </summary>
    public string? Cc { get; set; }

    /// <summary>
    /// Blind carbon copy recipients (comma-separated).
    /// </summary>
    public string? Bcc { get; set; }
}
