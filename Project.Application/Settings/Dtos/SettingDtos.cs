namespace Project.Application.Settings.Dtos;

public class SettingDto
{
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
}

public class SetSettingDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class SettingValueDto
{
    public string Value { get; set; } = string.Empty;
}
