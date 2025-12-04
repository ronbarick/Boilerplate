

namespace Project.Emailing;

/// <summary>
/// File-based email template provider with placeholder replacement.
/// </summary>
public class EmailTemplateProvider : IEmailTemplateProvider
{
    private readonly string _templateBasePath;

    public EmailTemplateProvider(string templateBasePath = "EmailTemplates")
    {
        _templateBasePath = templateBasePath;
    }

    public async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string>? parameters = null)
    {
        var templatePath = Path.Combine(_templateBasePath, $"{templateName}.html");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Email template '{templateName}' not found at '{templatePath}'");
        }

        var template = await File.ReadAllTextAsync(templatePath);

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                template = template.Replace($"{{{{{parameter.Key}}}}}", parameter.Value);
            }
        }

        return template;
    }
}
