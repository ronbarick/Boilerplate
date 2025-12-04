namespace Project.Domain.Shared.Constants;

public static class SaaSSettingNames
{
    private const string Prefix = "SaaS";

    // Payment Settings
    public const string PaymentGateway = Prefix + ".Payment.Gateway"; // Razorpay or Stripe
    public const string Currency = Prefix + ".Payment.Currency";
    public const string EnableTestMode = Prefix + ".Payment.TestMode";
    
    // Invoicing
    public const string InvoiceAddress = Prefix + ".Invoice.Address";
    public const string TaxPercentage = Prefix + ".Invoice.TaxPercentage";
    public const string TaxName = Prefix + ".Invoice.TaxName"; // VAT, GST, etc.
    
    // Trial & Grace Period
    public const string DefaultTrialDays = Prefix + ".Subscription.DefaultTrialDays";
    public const string GracePeriodDays = Prefix + ".Subscription.GracePeriodDays";
    public const string MaxTrialExtensions = Prefix + ".Subscription.MaxTrialExtensions";
    
    // Alerts
    public const string UsageAlertThreshold = Prefix + ".Alerts.UsageThreshold"; // e.g. 80%
    public const string EnableUsageAlerts = Prefix + ".Alerts.EnableUsageAlerts";
    
    // Email
    public const string SendInvoiceEmail = Prefix + ".Email.SendInvoice";
    public const string SendPaymentFailedEmail = Prefix + ".Email.SendPaymentFailed";
    public const string SendTrialEndingEmail = Prefix + ".Email.SendTrialEnding";


    // Feature Toggles
    public const string IsEnabled = Prefix + ".Subscription.IsEnabled";
    public const string TenantIsEnabled = Prefix + ".Subscription.Tenant.IsEnabled";
}
