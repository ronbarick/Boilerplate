namespace Project.Domain.Shared.Exceptions;

public class SubscriptionPausedException : UserFriendlyException
{
    public SubscriptionPausedException()
        : base("Your subscription is currently paused. Please resume to continue.")
    {
    }
}
