using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Core.Entities;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Services;

/// <summary>
/// Domain service implementation for user management operations.
/// </summary>
public class UserManager : IUserManager
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserManager> _logger;
    private const int TokenExpirationHours = 24;

    public UserManager(
        IRepository<User> userRepository,
        ILogger<UserManager> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> CreateUserWithPasswordAsync(
        string userName,
        string name,
        string? surname,
        string emailAddress,
        string password,
        string? phoneNumber = null)
    {
        _logger.LogInformation("Creating user with password: {UserName}, {Email}", userName, emailAddress);
        
        try
        {
            // Check for duplicates
            var existingUser = await _userRepository.GetQueryable()
                .AnyAsync(u => u.UserName == userName || u.EmailAddress == emailAddress);
            
            if (existingUser)
            {
                _logger.LogWarning("Duplicate user creation attempt: {UserName}, {Email}", userName, emailAddress);
                throw new InvalidOperationException("Username or email already exists");
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Name = name,
                Surname = surname,
                EmailAddress = emailAddress,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                PhoneNumber = phoneNumber,
                IsActive = true,
                IsEmailConfirmed = false,
                ShouldChangePasswordOnNextLogin = false,
                CreatedOn = DateTime.UtcNow
            };

            await _userRepository.InsertAsync(user);
            _logger.LogInformation("User created successfully with password: {UserId}, {UserName}", user.Id, userName);
            return user;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Failed to create user with password: {UserName}, {Email}", userName, emailAddress);
            throw;
        }
    }

    public async Task<(User user, string token)> CreateUserWithoutPasswordAsync(
        string userName,
        string name,
        string? surname,
        string emailAddress,
        string? phoneNumber = null)
    {
        _logger.LogInformation("Creating user without password: {UserName}, {Email}", userName, emailAddress);
        
        try
        {
            // Check for duplicates
            var existingUser = await _userRepository.GetQueryable()
                .AnyAsync(u => u.UserName == userName || u.EmailAddress == emailAddress);
            
            if (existingUser)
            {
                _logger.LogWarning("Duplicate user creation attempt: {UserName}, {Email}", userName, emailAddress);
                throw new InvalidOperationException("Username or email already exists");
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Name = name,
                Surname = surname,
                EmailAddress = emailAddress,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(GenerateRandomPassword()), // Temporary password
                PhoneNumber = phoneNumber,
                IsActive = true,
                IsEmailConfirmed = false,
                ShouldChangePasswordOnNextLogin = true,
                CreatedOn = DateTime.UtcNow
            };

            // Generate invitation token
            var token = GenerateSecureToken();
            user.InvitationToken = HashToken(token);
            user.TokenExpiration = DateTime.UtcNow.AddHours(TokenExpirationHours);

            await _userRepository.InsertAsync(user);
            _logger.LogInformation("User created successfully without password: {UserId}, {UserName}", user.Id, userName);
            
            return (user, token); // Return both user and plain token
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Failed to create user without password: {UserName}, {Email}", userName, emailAddress);
            throw;
        }
    }

    public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
    {
        _logger.LogInformation("Generating password reset token for user {UserId}", userId);
        
        try
        {
            var user = await _userRepository.GetAsync(userId);
            
            var token = GenerateSecureToken();
            user.InvitationToken = HashToken(token);
            user.TokenExpiration = DateTime.UtcNow.AddHours(TokenExpirationHours);

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password reset token generated successfully for user {UserId}", userId);
            
            return token; // Return plain token for email
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate password reset token for user {UserId}", userId);
            throw;
        }
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
    {
        _logger.LogInformation("Generating email confirmation token for user {UserId}", userId);
        
        try
        {
            var user = await _userRepository.GetAsync(userId);
            
            var token = GenerateSecureToken();
            user.EmailConfirmationToken = HashToken(token);
            user.TokenExpiration = DateTime.UtcNow.AddHours(TokenExpirationHours);

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Email confirmation token generated successfully for user {UserId}", userId);
            
            return token; // Return plain token for email
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate email confirmation token for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ValidatePasswordResetTokenAsync(Guid userId, string token)
    {
        try
        {
            var user = await _userRepository.GetAsync(userId);
            
            if (user.InvitationToken == null || user.TokenExpiration == null)
            {
                _logger.LogWarning("Password reset token validation failed: No token found for user {UserId}", userId);
                return false;
            }

            if (user.TokenExpiration < DateTime.UtcNow)
            {
                _logger.LogWarning("Password reset token validation failed: Token expired for user {UserId}", userId);
                return false;
            }

            var isValid = VerifyToken(token, user.InvitationToken);
            if (!isValid)
            {
                _logger.LogWarning("Password reset token validation failed: Invalid token for user {UserId}", userId);
            }
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password reset token for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ValidateEmailConfirmationTokenAsync(Guid userId, string token)
    {
        try
        {
            var user = await _userRepository.GetAsync(userId);
            
            if (user.EmailConfirmationToken == null || user.TokenExpiration == null)
            {
                _logger.LogWarning("Email confirmation token validation failed: No token found for user {UserId}", userId);
                return false;
            }

            if (user.TokenExpiration < DateTime.UtcNow)
            {
                _logger.LogWarning("Email confirmation token validation failed: Token expired for user {UserId}", userId);
                return false;
            }

            var isValid = VerifyToken(token, user.EmailConfirmationToken);
            if (!isValid)
            {
                _logger.LogWarning("Email confirmation token validation failed: Invalid token for user {UserId}", userId);
            }
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating email confirmation token for user {UserId}", userId);
            return false;
        }
    }

    public async Task SetPasswordAsync(Guid userId, string password)
    {
        _logger.LogInformation("Setting password for user {UserId}", userId);
        
        try
        {
            var user = await _userRepository.GetAsync(userId);
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.ShouldChangePasswordOnNextLogin = false;
            user.InvitationToken = null;
            user.TokenExpiration = null;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password set successfully for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set password for user {UserId}", userId);
            throw;
        }
    }

    public async Task ResetPasswordAsync(Guid userId, string token, string newPassword)
    {
        _logger.LogInformation("Resetting password for user {UserId}", userId);
        
        try
        {
            if (!await ValidatePasswordResetTokenAsync(userId, token))
            {
                _logger.LogWarning("Password reset failed: Invalid or expired token for user {UserId}", userId);
                throw new InvalidOperationException("Invalid or expired token");
            }

            var user = await _userRepository.GetAsync(userId);
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ShouldChangePasswordOnNextLogin = false;
            user.InvitationToken = null;
            user.TokenExpiration = null;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password reset successfully for user {UserId}", userId);
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Failed to reset password for user {UserId}", userId);
            throw;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Generates a cryptographically secure random token.
    /// </summary>
    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Hashes a token using BCrypt for secure storage.
    /// </summary>
    private static string HashToken(string token)
    {
        return BCrypt.Net.BCrypt.HashPassword(token);
    }

    /// <summary>
    /// Verifies a plain token against a hashed token.
    /// </summary>
    private static bool VerifyToken(string plainToken, string hashedToken)
    {
        return BCrypt.Net.BCrypt.Verify(plainToken, hashedToken);
    }

    /// <summary>
    /// Generates a random password for users created without a password.
    /// </summary>
    private static string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString("N");
    }

    #endregion
}
