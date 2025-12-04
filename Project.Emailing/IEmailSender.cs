namespace Project.Emailing;

/// <summary>
/// Service for sending emails.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email.
    /// </summary>
    Task SendAsync(string to, string subject, string body, bool isBodyHtml = true);

    /// <summary>
    /// Sends an email using EmailMessage.
    /// </summary>
    Task SendAsync(EmailMessage message);

    /// <summary>
    /// Sends an email with attachments.
    /// </summary>
    Task SendWithAttachmentsAsync(string to, string subject, string body, List<EmailAttachment> attachments, bool isBodyHtml = true);

    /// <summary>
    /// Queues an email for background sending.
    /// </summary>
    Task QueueAsync(string to, string subject, string body, bool isBodyHtml = true);

    /// <summary>
    /// Queues an email with attachments for background sending.
    /// </summary>
    Task QueueWithAttachmentsAsync(EmailMessage message);
}
