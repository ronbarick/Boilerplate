using Project.Domain.Settings;

namespace Project.Domain.Interfaces;

/// <summary>
/// Manages setting definitions (metadata).
/// </summary>
public interface ISettingDefinitionManager
{
    /// <summary>
    /// Gets a setting definition by name.
    /// </summary>
    SettingDefinition? GetOrNull(string name);

    /// <summary>
    /// Gets all setting definitions.
    /// </summary>
    IReadOnlyList<SettingDefinition> GetAll();
}
