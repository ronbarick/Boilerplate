using System.Collections.Generic;

namespace Project.Application.AppConfiguration.Dtos;

public class LocalizationTextsDto
{
    public string Culture { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public Dictionary<string, string> Texts { get; set; } = new();
}
