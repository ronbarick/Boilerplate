using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Project.Domain.Interfaces;
using Project.Domain.Sms;

namespace Project.Infrastructure.Sms;

/// <summary>
/// Twilio SMS provider implementation.
/// </summary>
public class TwilioSmsSender : ISmsSender
{
    private readonly ISettingManager _settingManager;
    private readonly ILogger<TwilioSmsSender> _logger;
    private readonly HttpClient _httpClient;

    public TwilioSmsSender(
        ISettingManager settingManager,
        ILogger<TwilioSmsSender> logger,
        IHttpClientFactory httpClientFactory)
    {
        _settingManager = settingManager;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task SendAsync(string phoneNumber, string message)
    {
        try
        {
            var accountSid = await _settingManager.GetOrNullAsync("Sms.Twilio.AccountSid");
            var authToken = await _settingManager.GetOrNullAsync("Sms.Twilio.AuthToken");
            var fromNumber = await _settingManager.GetOrNullAsync("Sms.Twilio.FromNumber");

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
            {
                throw new InvalidOperationException("Twilio SMS settings are not configured");
            }

            var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("To", phoneNumber),
                new KeyValuePair<string, string>("From", fromNumber),
                new KeyValuePair<string, string>("Body", message)
            });

            var response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Twilio SMS failed: {Error}", error);
                throw new Exception($"Failed to send SMS via Twilio: {error}");
            }

            _logger.LogInformation("SMS sent successfully to {PhoneNumber} via Twilio", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS via Twilio to {PhoneNumber}", phoneNumber);
            throw;
        }
    }
}
