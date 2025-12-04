namespace Project.Emailing;

/// <summary>
/// Provides email templates with placeholder replacement.
/// </summary>
public interface IEmailTemplateProvider
{
    /// <summary>
    /// Gets an email template and replaces placeholders.
    /// </summary>
    /// <param name="templateName">Name of the template (without .html extension)</param>
    /// <param name="parameters">Dictionary of placeholder values</param>
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string>? parameters = null);
}
