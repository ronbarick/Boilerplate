namespace Project.Domain.Shared.Exceptions;

public class CouponInvalidException : UserFriendlyException
{
    public CouponInvalidException(string message = "The provided coupon is invalid or expired.")
        : base(message)
    {
    }
}
