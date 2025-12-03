using System.Collections.Generic;

namespace Project.Application.AppConfiguration.Dtos;

public class FeatureListDto
{
    public List<FeatureDto> Features { get; set; } = new();
}

public class FeatureDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}
