namespace Project.Core.Interfaces;

public interface IFeatureChecker
{
    Task<bool> IsEnabledAsync(string featureName);
    Task<bool> IsEnabledAsync(Guid tenantId, string featureName);
    Task<string?> GetValueAsync(string featureName);
    Task<string?> GetValueAsync(Guid tenantId, string featureName);
}
