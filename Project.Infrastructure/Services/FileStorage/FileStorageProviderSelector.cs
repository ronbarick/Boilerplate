using Microsoft.Extensions.Configuration;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// Service to select the appropriate storage provider based on configuration
/// </summary>
public class FileStorageProviderSelector : IFileStorageProviderSelector
{
    private readonly IConfiguration _configuration;
    private readonly IEnumerable<IFileStorageProvider> _providers;

    public FileStorageProviderSelector(
        IConfiguration configuration,
        IEnumerable<IFileStorageProvider> providers)
    {
        _configuration = configuration;
        _providers = providers;
    }

    public IFileStorageProvider GetProvider()
    {
        var providerName = _configuration["App:FileStorage:Provider"] ?? "Local";
        return GetProvider(providerName);
    }

    public IFileStorageProvider GetProvider(string providerName)
    {
        var provider = _providers.FirstOrDefault(p => 
            p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            throw new InvalidOperationException($"Storage provider '{providerName}' is not registered.");
        }

        return provider;
    }
}
