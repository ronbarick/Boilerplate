
using Project.Domain.Shared.Interfaces;

namespace Project.Emailing;

/// <summary>
/// Resolves SMTP configuration from ISettingManager with tenant override support.
/// </summary>
public class EmailSenderConfiguration : IEmailSenderConfiguration
{
    private readonly ISettingManager _settingManager;
    private readonly ICurrentTenant _currentTenant;

    public EmailSenderConfiguration(ISettingManager settingManager, ICurrentTenant currentTenant)
    {
        _settingManager = settingManager;
        _currentTenant = currentTenant;
    }

    public async Task<SmtpConfiguration> GetAsync()
    {
        return new SmtpConfiguration
        {
            Host = await _settingManager.GetAsync("Email.Smtp.Host"),
            Port = int.Parse(await _settingManager.GetAsync("Email.Smtp.Port")),
            EnableSsl = bool.Parse(await _settingManager.GetAsync("Email.Smtp.EnableSsl")),
            UseDefaultCredentials = bool.Parse(await _settingManager.GetAsync("Email.Smtp.UseDefaultCredentials")),
            UserName = await _settingManager.GetOrNullAsync("Email.Smtp.UserName"),
            Password = await _settingManager.GetOrNullAsync("Email.Smtp.Password"),
            DefaultFromAddress = await _settingManager.GetAsync("Email.DefaultFromAddress"),
            DefaultFromDisplayName = await _settingManager.GetAsync("Email.DefaultFromDisplayName")
        };
    }

    public async Task<SmtpConfiguration> GetForTenantAsync(Guid tenantId)
    {
        return new SmtpConfiguration
        {
            Host = await _settingManager.GetForTenantAsync("Email.Smtp.Host", tenantId) ?? await _settingManager.GetAsync("Email.Smtp.Host"),
            Port = int.Parse(await _settingManager.GetForTenantAsync("Email.Smtp.Port", tenantId) ?? await _settingManager.GetAsync("Email.Smtp.Port")),
            EnableSsl = bool.Parse(await _settingManager.GetForTenantAsync("Email.Smtp.EnableSsl", tenantId) ?? await _settingManager.GetAsync("Email.Smtp.EnableSsl")),
            UseDefaultCredentials = bool.Parse(await _settingManager.GetForTenantAsync("Email.Smtp.UseDefaultCredentials", tenantId) ?? await _settingManager.GetAsync("Email.Smtp.UseDefaultCredentials")),
            UserName = await _settingManager.GetForTenantAsync("Email.Smtp.UserName", tenantId),
            Password = await _settingManager.GetForTenantAsync("Email.Smtp.Password", tenantId),
            DefaultFromAddress = await _settingManager.GetForTenantAsync("Email.DefaultFromAddress", tenantId) ?? await _settingManager.GetAsync("Email.DefaultFromAddress"),
            DefaultFromDisplayName = await _settingManager.GetForTenantAsync("Email.DefaultFromDisplayName", tenantId) ?? await _settingManager.GetAsync("Email.DefaultFromDisplayName")
        };
    }
}
