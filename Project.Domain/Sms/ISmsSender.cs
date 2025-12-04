namespace Project.Domain.Sms;

/// <summary>
/// Interface for sending SMS messages.
/// </summary>
public interface ISmsSender
{
    /// <summary>
    /// Sends an SMS message to the specified phone number.
    /// </summary>
    Task SendAsync(string phoneNumber, string message);
}
