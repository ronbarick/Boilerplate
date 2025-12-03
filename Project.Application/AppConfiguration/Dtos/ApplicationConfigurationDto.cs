using System.Collections.Generic;

namespace Project.Application.AppConfiguration.Dtos;

public class ApplicationConfigurationDto
{
    public LocalizationConfigurationDto Localization { get; set; } = new();
    public AuthConfigurationDto Auth { get; set; } = new();
    public MultiTenancyConfigurationDto MultiTenancy { get; set; } = new();
    public CurrentUserDto? CurrentUser { get; set; }
    public FeaturesConfigurationDto Features { get; set; } = new();
    public Dictionary<string, string> Settings { get; set; } = new();
}

public class LocalizationConfigurationDto
{
    public CurrentCultureDto CurrentCulture { get; set; } = new();
    public List<LanguageDto> Languages { get; set; } = new();
    public List<string> Resources { get; set; } = new();
}

public class CurrentCultureDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class LanguageDto
{
    public string CultureName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class AuthConfigurationDto
{
    public Dictionary<string, bool> GrantedPolicies { get; set; } = new();
    public bool TwoFactorEnabled { get; set; }
}

public class MultiTenancyConfigurationDto
{
    public bool IsEnabled { get; set; }
}

public class CurrentUserDto
{
    public bool IsAuthenticated { get; set; }
    public Guid? Id { get; set; }
    public Guid? TenantId { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class FeaturesConfigurationDto
{
    public Dictionary<string, string> Values { get; set; } = new();
}
