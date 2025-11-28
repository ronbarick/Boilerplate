using System.Net;
using System.Net.Mail;
using Project.Core.Emailing;
using Project.Core.BackgroundJobs;

namespace Project.Infrastructure.Emailing;

/// <summary>
/// SMTP-based email sender implementation.
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly IEmailSenderConfiguration _configuration;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public SmtpEmailSender(IEmailSenderConfiguration configuration, IBackgroundJobManager backgroundJobManager)
    {
        _configuration = configuration;
        _backgroundJobManager = backgroundJobManager;
    }

    public async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml
        };

        await SendAsync(message);
    }

    public async Task SendAsync(EmailMessage message)
    {
        var config = await _configuration.GetAsync();

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(config.DefaultFromAddress, config.DefaultFromDisplayName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsBodyHtml
        };

        mailMessage.To.Add(message.To);

        // Add CC
        if (!string.IsNullOrWhiteSpace(message.Cc))
        {
            foreach (var cc in message.Cc.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.CC.Add(cc.Trim());
            }
        }

        // Add BCC
        if (!string.IsNullOrWhiteSpace(message.Bcc))
        {
            foreach (var bcc in message.Bcc.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.Bcc.Add(bcc.Trim());
            }
        }

        // Add attachments
        foreach (var attachment in message.Attachments)
        {
            var stream = new MemoryStream(attachment.Content);
            mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
        }

        using var smtpClient = new SmtpClient(config.Host, config.Port)
        {
            EnableSsl = config.EnableSsl,
            UseDefaultCredentials = config.UseDefaultCredentials
        };

        if (!config.UseDefaultCredentials && !string.IsNullOrEmpty(config.UserName))
        {
            smtpClient.Credentials = new NetworkCredential(config.UserName, config.Password);
        }

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendWithAttachmentsAsync(string to, string subject, string body, List<EmailAttachment> attachments, bool isBodyHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml,
            Attachments = attachments
        };

        await SendAsync(message);
    }

    public async Task QueueAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml
        };

        await QueueWithAttachmentsAsync(message);
    }

    public async Task QueueWithAttachmentsAsync(EmailMessage message)
    {
        // Enqueue the job to call SendAsync on IEmailSender
        // Note: We use Enqueue<IEmailSender> so Hangfire resolves the interface implementation
        _backgroundJobManager.Enqueue<IEmailSender>(x => x.SendAsync(message));
        await Task.CompletedTask;
    }
}
