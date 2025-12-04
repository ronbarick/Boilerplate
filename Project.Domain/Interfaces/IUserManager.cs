using System;
using System.Threading.Tasks;
using Project.Domain.Entities;

namespace Project.Domain.Interfaces;

/// <summary>
/// Domain service for user management operations including password and token management.
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Creates a new user with a password.
    /// </summary>
    Task<User> CreateUserWithPasswordAsync(
        string userName, 
        string name, 
        string? surname, 
        string emailAddress, 
        string password, 
        string? phoneNumber = null);

    /// <summary>
    /// Creates a new user without a password. User must set password on first login.
    /// </summary>
    /// <returns>Tuple containing the created user and the plain invitation token</returns>
    Task<(User user, string token)> CreateUserWithoutPasswordAsync(
        string userName, 
        string name, 
        string? surname, 
        string emailAddress, 
        string? phoneNumber = null);

    /// <summary>
    /// Generates a password reset token for the specified user.
    /// </summary>
    Task<string> GeneratePasswordResetTokenAsync(Guid userId);

    /// <summary>
    /// Generates an email confirmation token for the specified user.
    /// </summary>
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);

    /// <summary>
    /// Validates a password reset token for the specified user.
    /// </summary>
    Task<bool> ValidatePasswordResetTokenAsync(Guid userId, string token);

    /// <summary>
    /// Validates an email confirmation token for the specified user.
    /// </summary>
    Task<bool> ValidateEmailConfirmationTokenAsync(Guid userId, string token);

    /// <summary>
    /// Sets the password for a user (used when setting initial password).
    /// </summary>
    Task SetPasswordAsync(Guid userId, string password);

    /// <summary>
    /// Resets the password for a user using a valid token.
    /// </summary>
    Task ResetPasswordAsync(Guid userId, string token, string newPassword);
}
