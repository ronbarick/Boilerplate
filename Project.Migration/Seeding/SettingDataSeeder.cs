using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Core.Entities;
using Project.Infrastructure.Data;

namespace Project.Migration.Seeding;

public static class SettingDataSeeder
{
    public static void SeedSettings(AppDbContext context, IConfiguration configuration)
    {
        // 1. Get SMTP settings from configuration (appsettings.json or Env Vars)
        var smtpHost = configuration["Seed:Email:Smtp:Host"] ?? "smtp.gmail.com";
        var smtpPort = configuration["Seed:Email:Smtp:Port"] ?? "587";
        var smtpUserName = configuration["Seed:Email:Smtp:UserName"] ?? "";
        var smtpPassword = configuration["Seed:Email:Smtp:Password"] ?? "";
        var defaultFromAddress = configuration["Seed:Email:DefaultFromAddress"] ?? "noreply@app.com";
        var defaultDisplayName = configuration["Seed:Email:DefaultFromDisplayName"] ?? "Application";

        // 2. Define settings to seed
        var settingsToSeed = new Dictionary<string, string>
        {
            { "Email.Smtp.Host", smtpHost },
            { "Email.Smtp.Port", smtpPort },
            { "Email.Smtp.UserName", smtpUserName },
            { "Email.Smtp.Password", smtpPassword },
            { "Email.Smtp.EnableSsl", "true" },
            { "Email.Smtp.UseDefaultCredentials", "false" },
            { "Email.DefaultFromAddress", defaultFromAddress },
            { "Email.DefaultFromDisplayName", defaultDisplayName },
            { "Security.TwoFactor.Enabled", "false" },
            { "Security.TwoFactor.MandatoryForRoles", "" },
            { "Sms.Provider", "Twilio" },
            { "Sms.Twilio.AccountSid", "" },
            { "Sms.Twilio.AuthToken", "" },
            { "Sms.Twilio.FromNumber", "" },
            { "Sms.Msg91.AuthKey", "" },
            { "Sms.Msg91.SenderId", "" },
            { "Sms.Msg91.Route", "4" }
        };

        // 3. Insert settings if they don't exist
        foreach (var setting in settingsToSeed)
        {
            // Check if global setting exists (ProviderName = "G", ProviderKey = null)
            var exists = context.Settings.Any(s => 
                s.Name == setting.Key && 
                s.ProviderName == "G" && 
                s.ProviderKey == null);

            if (!exists)
            {
                context.Settings.Add(new Setting
                {
                    Id = Guid.NewGuid(),
                    Name = setting.Key,
                    Value = setting.Value,
                    ProviderName = "G",
                    ProviderKey = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                });
            }
        }

        context.SaveChanges();
    }
}
