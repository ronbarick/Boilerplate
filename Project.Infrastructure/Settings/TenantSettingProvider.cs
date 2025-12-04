using Project.Domain.Interfaces;

namespace Project.Infrastructure.Settings;

/// <summary>
/// Provides tenant-specific settings (ProviderName = "T", ProviderKey = tenantId).
/// </summary>
public class TenantSettingProvider : ISettingProvider
{
    private readonly ISettingStore _settingStore;
    private readonly ICurrentTenant _currentTenant;

    public string Name => "T";

    public TenantSettingProvider(ISettingStore settingStore, ICurrentTenant currentTenant)
    {
        _settingStore = settingStore;
        _currentTenant = currentTenant;
    }

    public Task<string?> GetOrNullAsync(string settingName)
    {
        if (!_currentTenant.Id.HasValue)
        {
            return Task.FromResult<string?>(null);
        }

        return _settingStore.GetOrNullAsync(settingName, Name, _currentTenant.Id.Value.ToString());
    }
}
