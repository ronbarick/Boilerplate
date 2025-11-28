using System;

namespace Project.Application.Users.Dtos;

/// <summary>
/// DTO for forgot password request.
/// </summary>
public class ForgotPasswordDto
{
    public string EmailAddress { get; set; } = null!;
}

/// <summary>
/// DTO for resetting password with token.
/// </summary>
public class ResetPasswordDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

/// <summary>
/// DTO for setting password with token (first-time password setup).
/// </summary>
public class SetPasswordDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

/// <summary>
/// DTO for confirming email address.
/// </summary>
public class ConfirmEmailDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
}
