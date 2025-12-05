using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Project.Domain.Entities;
using Project.Domain.Interfaces;

namespace Project.Application.BackgroundJobs.Email;

/// <summary>
/// Background job for sending set password emails to users created without passwords.
/// </summary>
public class SendSetPasswordEmailJob
{
    private readonly IRepository<User> _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ILogger<SendSetPasswordEmailJob> _logger;

    public SendSetPasswordEmailJob(
        IRepository<User> userRepository,
        IEmailSender emailSender,
        IEmailTemplateProvider emailTemplateProvider,
        ILogger<SendSetPasswordEmailJob> logger)
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
            _logger.LogInformation("Sending set-password email for user {UserId}", userId);
            
            var user = await _userRepository.GetAsync(userId);

            var encodedToken = WebUtility.UrlEncode(token);
            var setPasswordLink = $"/account/set-password?userId={userId}&token={encodedToken}";

            var parameters = new Dictionary<string, string>
            {
                { "Name", user.Name },
                { "SetPasswordLink", setPasswordLink },
                { "ExpirationHours", "24" }
            };

            var body = await _emailTemplateProvider.GetTemplateAsync("SetPasswordEmail", parameters);

            await _emailSender.QueueAsync(
                user.EmailAddress,
                "Set Your Password",
                body,
                isBodyHtml: true);
                
            _logger.LogInformation("Set-password email queued successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send set-password email for user {UserId}", userId);
            throw;
        }
    }
}
