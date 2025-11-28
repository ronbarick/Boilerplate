using Project.Core.Interfaces;

namespace Project.Infrastructure.Settings;

/// <summary>
/// Provides user-specific settings (ProviderName = "U", ProviderKey = userId).
/// </summary>
public class UserSettingProvider : ISettingProvider
{
    private readonly ISettingStore _settingStore;
    private readonly ICurrentUser _currentUser;

    public string Name => "U";

    public UserSettingProvider(ISettingStore settingStore, ICurrentUser currentUser)
    {
        _settingStore = settingStore;
        _currentUser = currentUser;
    }

    public Task<string?> GetOrNullAsync(string settingName)
    {
        if (!_currentUser.Id.HasValue)
        {
            return Task.FromResult<string?>(null);
        }

        return _settingStore.GetOrNullAsync(settingName, Name, _currentUser.Id.Value.ToString());
    }
}
