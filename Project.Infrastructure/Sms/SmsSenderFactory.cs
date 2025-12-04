using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Project.Domain.Interfaces;
using Project.Domain.Sms;

namespace Project.Infrastructure.Sms;

/// <summary>
/// Factory to resolve the active SMS provider based on settings.
/// </summary>
public class SmsSenderFactory
{
    private readonly ISettingManager _settingManager;
    private readonly IServiceProvider _serviceProvider;

    public SmsSenderFactory(ISettingManager settingManager, IServiceProvider serviceProvider)
    {
        _settingManager = settingManager;
        _serviceProvider = serviceProvider;
    }

    public async Task<ISmsSender> GetSenderAsync()
    {
        var provider = await _settingManager.GetOrNullAsync("Sms.Provider");

        return provider?.ToLower() switch
        {
            "twilio" => _serviceProvider.GetRequiredService<TwilioSmsSender>(),
            "msg91" => _serviceProvider.GetRequiredService<Msg91SmsSender>(),
            _ => throw new InvalidOperationException($"SMS provider '{provider}' is not configured or supported. Please set 'Sms.Provider' to 'Twilio' or 'Msg91'.")
        };
    }
}
