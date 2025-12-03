using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Application.BackgroundJobs;

/// <summary>
/// Background job for sending password reset emails.
/// </summary>
public class SendResetPasswordEmailJob
{
    private readonly IRepository<User> _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ILogger<SendResetPasswordEmailJob> _logger;

    public SendResetPasswordEmailJob(
        IRepository<User> userRepository,
        IEmailSender emailSender,
        IEmailTemplateProvider emailTemplateProvider,
        ILogger<SendResetPasswordEmailJob> logger)
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
            _logger.LogInformation("Sending reset-password email for user {UserId}", userId);
            
            var user = await _userRepository.GetAsync(userId);

            var encodedToken = WebUtility.UrlEncode(token);
            var resetPasswordLink = $"/account/reset-password?userId={userId}&token={encodedToken}";

            var parameters = new Dictionary<string, string>
            {
                { "Name", user.Name },
                { "ResetPasswordLink", resetPasswordLink },
                { "ExpirationHours", "24" }
            };

            var body = await _emailTemplateProvider.GetTemplateAsync("ResetPasswordEmail", parameters);

            await _emailSender.QueueAsync(
                user.EmailAddress,
                "Reset Your Password",
                body,
                isBodyHtml: true);
                
            _logger.LogInformation("Reset-password email queued successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send reset-password email for user {UserId}", userId);
            throw;
        }
    }
}
