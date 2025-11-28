using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Project.Core.Emailing;
using Project.Core.Entities;
using Project.Core.Interfaces;

namespace Project.Application.BackgroundJobs;

/// <summary>
/// Background job for sending welcome emails to new users.
/// </summary>
public class SendWelcomeEmailJob
{
    private readonly IRepository<User> _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ILogger<SendWelcomeEmailJob> _logger;

    public SendWelcomeEmailJob(
        IRepository<User> userRepository,
        IEmailSender emailSender,
        IEmailTemplateProvider emailTemplateProvider,
        ILogger<SendWelcomeEmailJob> logger)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Sending welcome email for user {UserId}", userId);
            
            var user = await _userRepository.GetAsync(userId);

            var parameters = new Dictionary<string, string>
            {
                { "Name", user.Name },
                { "EmailAddress", user.EmailAddress }
            };

            var body = await _emailTemplateProvider.GetTemplateAsync("WelcomeEmail", parameters);

            await _emailSender.QueueAsync(
                user.EmailAddress,
                "Welcome to Our Platform",
                body,
                isBodyHtml: true);
                
            _logger.LogInformation("Welcome email queued successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email for user {UserId}", userId);
            throw; // Re-throw to mark job as failed in Hangfire
        }
    }
}
