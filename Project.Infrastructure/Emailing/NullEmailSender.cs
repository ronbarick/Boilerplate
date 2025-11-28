using Project.Core.Emailing;

namespace Project.Infrastructure.Emailing;

/// <summary>
/// Null email sender for testing/development (does nothing).
/// </summary>
public class NullEmailSender : IEmailSender
{
    public Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task SendAsync(EmailMessage message)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task SendWithAttachmentsAsync(string to, string subject, string body, List<EmailAttachment> attachments, bool isBodyHtml = true)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task QueueAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task QueueWithAttachmentsAsync(EmailMessage message)
    {
        // Do nothing
        return Task.CompletedTask;
    }
}
