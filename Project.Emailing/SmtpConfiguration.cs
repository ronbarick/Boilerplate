namespace Project.Emailing;

/// <summary>
/// SMTP server configuration.
/// </summary>
public class SmtpConfiguration
{
    /// <summary>
    /// SMTP server host.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Enable SSL/TLS.
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Use default credentials.
    /// </summary>
    public bool UseDefaultCredentials { get; set; }

    /// <summary>
    /// SMTP username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// SMTP password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Default sender email address.
    /// </summary>
    public string DefaultFromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Default sender display name.
    /// </summary>
    public string DefaultFromDisplayName { get; set; } = string.Empty;
}
