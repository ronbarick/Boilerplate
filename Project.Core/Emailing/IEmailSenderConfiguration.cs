namespace Project.Core.Emailing;

/// <summary>
/// Resolves SMTP configuration from settings.
/// </summary>
public interface IEmailSenderConfiguration
{
    /// <summary>
    /// Gets SMTP configuration for the current context (tenant or global).
    /// </summary>
    Task<SmtpConfiguration> GetAsync();

    /// <summary>
    /// Gets SMTP configuration for a specific tenant.
    /// </summary>
    Task<SmtpConfiguration> GetForTenantAsync(Guid tenantId);
}
