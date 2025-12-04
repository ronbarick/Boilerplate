namespace Project.Domain.Shared.Constants;

/// <summary>
/// Setting names for application configuration.
/// </summary>
public static class SettingNames
{
    // Two-Factor Authentication
    public const string TwoFactorEnabled = "Security.TwoFactor.Enabled";
    public const string TwoFactorMandatoryForRoles = "Security.TwoFactor.MandatoryForRoles";
    public const string PreferredTwoFactorProvider = "Security.TwoFactor.PreferredProvider"; // "Email", "Sms", "Authenticator"

    // SMS Settings
    public const string SmsProvider = "Sms.Provider"; // "Twilio" or "Msg91"
    public const string SmsTwilioAccountSid = "Sms.Twilio.AccountSid";
    public const string SmsTwilioAuthToken = "Sms.Twilio.AuthToken";
    public const string SmsTwilioFromNumber = "Sms.Twilio.FromNumber";
    public const string SmsMsg91AuthKey = "Sms.Msg91.AuthKey";
    public const string SmsMsg91SenderId = "Sms.Msg91.SenderId";
    public const string SmsMsg91Route = "Sms.Msg91.Route";
}
