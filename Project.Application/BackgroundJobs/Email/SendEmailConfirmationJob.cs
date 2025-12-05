using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Application.BackgroundJobs.Email;

/// <summary>
/// Background job for sending email confirmation links.
/// </summary>
public class SendEmailConfirmationJob
{
    private readonly IRepository<User> _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ILogger<SendEmailConfirmationJob> _logger;

    public SendEmailConfirmationJob(
        IRepository<User> userRepository,
        IEmailSender emailSender,
        IEmailTemplateProvider emailTemplateProvider,
        ILogger<SendEmailConfirmationJob> logger)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid userId, string token)
    {
        try
        {
            _logger.LogInformation("Sending email confirmation for user {UserId}", userId);
            
            var user = await _userRepository.GetAsync(userId);

            var encodedToken = WebUtility.UrlEncode(token);
            var confirmationLink = $"/account/confirm-email?userId={userId}&token={encodedToken}";

            var parameters = new Dictionary<string, string>
            {
                { "Name", user.Name },
                { "ConfirmationLink", confirmationLink },
                { "ExpirationHours", "24" }
            };

            var body = await _emailTemplateProvider.GetTemplateAsync("EmailConfirmation", parameters);

            await _emailSender.QueueAsync(
                user.EmailAddress,
                "Confirm Your Email",
                body,
                isBodyHtml: true);
                
            _logger.LogInformation("Email confirmation queued successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email confirmation for user {UserId}", userId);
            throw;
        }
    }
}
