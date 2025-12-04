namespace Project.Domain.Shared.Exceptions;

public class SubscriptionExpiredException : UserFriendlyException
{
    public SubscriptionExpiredException()
        : base("Your subscription has expired. Please renew to continue using the service.")
    {
    }
}
