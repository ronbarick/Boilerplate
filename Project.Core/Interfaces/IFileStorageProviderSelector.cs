namespace Project.Core.Interfaces;

/// <summary>
/// Service to select the appropriate storage provider based on configuration
/// </summary>
public interface IFileStorageProviderSelector
{
    /// <summary>
    /// Get the configured default storage provider
    /// </summary>
    IFileStorageProvider GetProvider();

    /// <summary>
    /// Get a specific storage provider by name
    /// </summary>
    IFileStorageProvider GetProvider(string providerName);
}
