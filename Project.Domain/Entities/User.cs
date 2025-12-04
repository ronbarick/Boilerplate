using System;
using System.Collections.Generic;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities;

public class User : FullAuditedEntity
{
    public string UserName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Surname { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; } = false;
    public Guid? ProfilePictureId { get; set; }
    public bool ShouldChangePasswordOnNextLogin { get; set; } = false;
    public DateTime? LastLoginTime { get; set; }
    public Guid? ConcurrencyStamp { get; set; }
    
    // Two-Factor Authentication
    public string? AuthenticatorKey { get; set; } // Encrypted TOTP secret
    public bool IsAuthenticatorEnabled { get; set; } = false;
    
    // Token fields for password reset and email confirmation
    public string? InvitationToken { get; set; }
    public DateTime? TokenExpiration { get; set; }
    public string? EmailConfirmationToken { get; set; }
    
    // Navigation properties
    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<UserPermission>? UserPermissions { get; set; }
}
