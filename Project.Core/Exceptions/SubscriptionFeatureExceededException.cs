namespace Project.Core.Exceptions;

public class SubscriptionFeatureExceededException : UserFriendlyException
{
    public SubscriptionFeatureExceededException(string featureName)
        : base($"The limit for feature '{featureName}' has been exceeded. Please upgrade your plan.")
    {
        Data["FeatureName"] = featureName;
    }
}
