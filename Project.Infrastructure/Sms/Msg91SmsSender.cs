using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Project.Core.Interfaces;
using Project.Core.Sms;

namespace Project.Infrastructure.Sms;

/// <summary>
/// MSG91 SMS provider implementation.
/// </summary>
public class Msg91SmsSender : ISmsSender
{
    private readonly ISettingManager _settingManager;
    private readonly ILogger<Msg91SmsSender> _logger;
    private readonly HttpClient _httpClient;

    public Msg91SmsSender(
        ISettingManager settingManager,
        ILogger<Msg91SmsSender> logger,
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
            var authKey = await _settingManager.GetOrNullAsync("Sms.Msg91.AuthKey");
            var senderId = await _settingManager.GetOrNullAsync("Sms.Msg91.SenderId");
            var route = await _settingManager.GetOrNullAsync("Sms.Msg91.Route") ?? "4";

            if (string.IsNullOrEmpty(authKey) || string.IsNullOrEmpty(senderId))
            {
                throw new InvalidOperationException("MSG91 SMS settings are not configured");
            }

            var url = "https://api.msg91.com/api/v5/flow/";

            var payload = new
            {
                sender = senderId,
                route = route,
                country = "91", // India country code, make configurable if needed
                sms = new[]
                {
                    new
                    {
                        message = new[] { message },
                        to = new[] { phoneNumber }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("authkey", authKey);

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("MSG91 SMS failed: {Error}", error);
                throw new Exception($"Failed to send SMS via MSG91: {error}");
            }

            _logger.LogInformation("SMS sent successfully to {PhoneNumber} via MSG91", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS via MSG91 to {PhoneNumber}", phoneNumber);
            throw;
        }
    }
}
