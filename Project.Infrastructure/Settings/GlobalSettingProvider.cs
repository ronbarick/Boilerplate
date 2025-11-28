using Project.Core.Interfaces;

namespace Project.Infrastructure.Settings;

/// <summary>
/// Provides global settings (ProviderName = "G", ProviderKey = null).
/// </summary>
public class GlobalSettingProvider : ISettingProvider
{
    private readonly ISettingStore _settingStore;

    public string Name => "G";

    public GlobalSettingProvider(ISettingStore settingStore)
    {
        _settingStore = settingStore;
    }

    public Task<string?> GetOrNullAsync(string settingName)
    {
        return _settingStore.GetOrNullAsync(settingName, Name, null);
    }
}
