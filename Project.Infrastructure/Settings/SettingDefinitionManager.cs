using Project.Domain.Interfaces;
using Project.Domain.Localization;
using Project.Domain.Settings;

namespace Project.Infrastructure.Settings;

/// <summary>
/// Manages setting definitions (in-memory).
/// </summary>
public class SettingDefinitionManager : ISettingDefinitionManager
{
    private readonly Dictionary<string, SettingDefinition> _definitions;

    public SettingDefinitionManager()
    {
        _definitions = new Dictionary<string, SettingDefinition>();
        InitializeDefinitions();
    }

    public SettingDefinition? GetOrNull(string name)
    {
        return _definitions.GetValueOrDefault(name);
    }

    public IReadOnlyList<SettingDefinition> GetAll()
    {
        return _definitions.Values.ToList();
    }

    private void InitializeDefinitions()
    {
        // Application Settings
        AddDefinition(new SettingDefinition(
            "App.Theme",
            "light",
            SettingScopes.User | SettingScopes.Tenant,
            "Application Theme",
            "The theme of the application (light or dark)"
        ));

        AddDefinition(new SettingDefinition(
            "App.Language",
            "en",
            SettingScopes.User | SettingScopes.Tenant,
            "Application Language",
            "The default language of the application"
        ));

        AddDefinition(new SettingDefinition(
            "App.MaxUploadSize",
            "10485760", // 10 MB
            SettingScopes.Global,
            "Maximum Upload Size",
            "Maximum file upload size in bytes"
        ));

        AddDefinition(new SettingDefinition(
            "App.MaintenanceMode",
            "false",
            SettingScopes.Global,
            "Maintenance Mode",
            "Whether the application is in maintenance mode"
        ));

        AddDefinition(new SettingDefinition(
            "App.TimeZone",
            "UTC",
            SettingScopes.User | SettingScopes.Tenant,
            "Time Zone",
            "The time zone for displaying dates and times"
        ));

        AddDefinition(new SettingDefinition(
            "App.DateFormat",
            "yyyy-MM-dd",
            SettingScopes.User | SettingScopes.Tenant,
            "Date Format",
            "The format for displaying dates"
        ));

        // Localization Settings
        AddDefinition(new SettingDefinition(
            LocalizationSettingNames.CurrentCulture,
            "en",
            SettingScopes.User | SettingScopes.Tenant | SettingScopes.Global,
            "Current Culture",
            "The current UI culture for the user/tenant"
        ));

        AddDefinition(new SettingDefinition(
            LocalizationSettingNames.SupportedCultures,
            "en,fr",
            SettingScopes.Global,
            "Supported Cultures",
            "Comma-separated list of supported cultures"
        ));

        AddDefinition(new SettingDefinition(
            LocalizationSettingNames.DefaultCulture,
            "en",
            SettingScopes.Global,
            "Default Culture",
            "Default culture when no other culture is specified"
        ));

        // Email Settings
        AddDefinition(new SettingDefinition(
            "Email.DefaultFromAddress",
            "noreply@app.com",
            SettingScopes.Global | SettingScopes.Tenant,
            "Default From Address",
            "Default email sender address"
        ));

        AddDefinition(new SettingDefinition(
            "Email.DefaultFromDisplayName",
            "Application",
            SettingScopes.Global | SettingScopes.Tenant,
            "Default From Display Name",
            "Default email sender display name"
        ));

        AddDefinition(new SettingDefinition(
            "Email.Smtp.Host",
            "smtp.gmail.com",
            SettingScopes.Global | SettingScopes.Tenant,
            "SMTP Host",
            "SMTP server hostname"
        ));

        AddDefinition(new SettingDefinition(
            "Email.Smtp.Port",
            "587",
            SettingScopes.Global | SettingScopes.Tenant,
            "SMTP Port",
            "SMTP server port"
        ));

        AddDefinition(new SettingDefinition(
            "Email.Smtp.EnableSsl",
            "true",
            SettingScopes.Global | SettingScopes.Tenant,
            "Enable SSL",
            "Enable SSL/TLS for SMTP"
        ));

        AddDefinition(new SettingDefinition(
            "Email.Smtp.UseDefaultCredentials",
            "false",
            SettingScopes.Global | SettingScopes.Tenant,
            "Use Default Credentials",
            "Use default Windows credentials"
        ));

        AddDefinition(new SettingDefinition(
            "Email.Smtp.UserName",
            "",
            SettingScopes.Global | SettingScopes.Tenant,
            "SMTP Username",
            "SMTP authentication username"
        ));

        AddDefinition(new SettingDefinition(
            "Email.Smtp.Password",
            "",
            SettingScopes.Global | SettingScopes.Tenant,
            "SMTP Password",
            "SMTP authentication password"
        ));
    }

    private void AddDefinition(SettingDefinition definition)
    {
        _definitions[definition.Name] = definition;
    }
}
