using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Dtos;
using Project.Application.TwoFactor.Dtos;
using Project.Core.Entities;
using Project.Core.Emailing;
using Project.Core.Interfaces;
using Project.Infrastructure.Emailing.Templates;
using Project.WebApi.Helpers;
using Project.WebApi.Services;

namespace Project.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("Login");

        group.MapPost("/verify-otp", VerifyOtpAsync)
            .AllowAnonymous()
            .WithName("VerifyOtp");

        group.MapPost("/resend-otp", ResendOtpAsync)
            .AllowAnonymous()
            .WithName("ResendOtp");
    }

    private static async Task<IResult> LoginAsync(
        LoginDto input,
        IRepository<User> userRepository,
        IRepository<AuditLog> auditRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        ITwoFactorService twoFactorService,
        IEmailSender emailSender,
        JwtTokenService jwtService,
        IAuditContext auditContext,
        HttpContext httpContext)
    {
        // Find user by username or email, disable query filters
        var user = await userRepository.GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserName == input.UserName || u.EmailAddress == input.UserName);

        if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash))
        {
            // Log failed login attempt
            await auditRepository.InsertAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user?.Id,
                TenantId = user?.TenantId,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = "AuthEndpoints",
                MethodName = "LoginAsync",
                ClientIpAddress = auditContext.ClientIpAddress,
                BrowserInfo = auditContext.BrowserInfo,
                CustomData = $"Failed login attempt for username: {input.UserName}"
            });

            return Results.Unauthorized();
        }

        if (!user.IsActive)
        {
            // Log inactive account login attempt
            await auditRepository.InsertAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TenantId = user.TenantId,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = "AuthEndpoints",
                MethodName = "LoginAsync",
                ClientIpAddress = auditContext.ClientIpAddress,
                BrowserInfo = auditContext.BrowserInfo,
                CustomData = $"Login attempt for inactive account: {user.UserName}"
            });

            return Results.Problem("User account is inactive", statusCode: 403);
        }

        // Check if user must change password
        if (user.ShouldChangePasswordOnNextLogin)
        {
            await auditRepository.InsertAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TenantId = user.TenantId,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = "AuthEndpoints",
                MethodName = "LoginAsync",
                ClientIpAddress = auditContext.ClientIpAddress,
                BrowserInfo = auditContext.BrowserInfo,
                CustomData = "Login blocked - password change required"
            });

            return Results.Json(
                new
                {
                    requiresPasswordChange = true,
                    message = "You must change your password before logging in"
                },
                statusCode: 403);
        }

        // Get user roles
        var userRoles = await userRoleRepository.GetQueryable()
            .Where(ur => ur.UserId == user.Id)
            .Join(roleRepository.GetQueryable(),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToArrayAsync();

        // Check if 2FA is required
        var deviceFingerprint = DeviceFingerprintHelper.GenerateFingerprint(httpContext);
        var requires2FA = await twoFactorService.IsTwoFactorRequiredAsync(
            user.Id, 
            user.TenantId, 
            userRoles, 
            deviceFingerprint);

        if (requires2FA)
        {
            // Check rate limiting
            var canRequestOtp = await twoFactorService.CanRequestOtpAsync(user.Id);
            if (!canRequestOtp)
            {
                return Results.Problem(
                    "Too many OTP requests. Please try again later.",
                    statusCode: 429);
            }

            // Generate and send OTP
            var otpCode = await twoFactorService.GenerateOtpAsync(user.Id);
            
            var emailBody = TwoFactorEmailTemplate.GenerateOtpEmail(otpCode, 5);
            await emailSender.SendAsync(user.EmailAddress, "Your Verification Code", emailBody, isBodyHtml: true);

            // Log 2FA required
            await auditRepository.InsertAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TenantId = user.TenantId,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = "AuthEndpoints",
                MethodName = "LoginAsync",
                ClientIpAddress = auditContext.ClientIpAddress,
                BrowserInfo = auditContext.BrowserInfo,
                CustomData = "Two-factor authentication required - OTP sent"
            });

            return Results.Ok(new
            {
                requiresTwoFactor = true,
                userId = user.Id,
                message = "Verification code sent to your email"
            });
        }

        // Generate tokens
        var token = jwtService.GenerateToken(
            user.Id,
            user.UserName,
            user.EmailAddress,
            user.TenantId,
            userRoles
        );

        var refreshToken = jwtService.GenerateRefreshToken();

        // Update last login time
        user.LastLoginTime = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        // Log successful login
        await auditRepository.InsertAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = user.TenantId,
            ExecutionTime = DateTime.UtcNow,
            ServiceName = "AuthEndpoints",
            MethodName = "LoginAsync",
            ClientIpAddress = auditContext.ClientIpAddress,
            BrowserInfo = auditContext.BrowserInfo,
            CustomData = "User logged in successfully"
        });

        return Results.Ok(new
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresIn = 28800 // 8 hours in seconds
        });
    }

    private static async Task<IResult> VerifyOtpAsync(
        VerifyOtpDto input,
        IRepository<User> userRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        IRepository<AuditLog> auditRepository,
        ITwoFactorService twoFactorService,
        JwtTokenService jwtService,
        IAuditContext auditContext,
        HttpContext httpContext)
    {
        // Get user
        var user = await userRepository.GetAsync(input.UserId);
        if (user == null)
            return Results.Unauthorized();

        // Check rate limiting
        var canVerify = await twoFactorService.CanVerifyOtpAsync(input.UserId);
        if (!canVerify)
        {
            await auditRepository.InsertAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TenantId = user.TenantId,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = "AuthEndpoints",
                MethodName = "VerifyOtpAsync",
                ClientIpAddress = auditContext.ClientIpAddress,
                BrowserInfo = auditContext.BrowserInfo,
                CustomData = "OTP verification blocked - too many attempts"
            });

            return Results.Problem(
                "Too many failed attempts. Please request a new code.",
                statusCode: 429);
        }

        // Validate OTP or backup code
        var isValid = await twoFactorService.ValidateOtpAsync(input.UserId, input.Code);
        
        if (!isValid)
        {
            // Try backup code
            isValid = await twoFactorService.ValidateBackupCodeAsync(input.UserId, input.Code);
        }

        if (!isValid)
        {
            await auditRepository.InsertAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TenantId = user.TenantId,
                ExecutionTime = DateTime.UtcNow,
                ServiceName = "AuthEndpoints",
                MethodName = "VerifyOtpAsync",
                ClientIpAddress = auditContext.ClientIpAddress,
                BrowserInfo = auditContext.BrowserInfo,
                CustomData = "Invalid OTP code entered"
            });

            return Results.Json(
                new { message = "Invalid or expired verification code" },
                statusCode: 401);
        }

        // Trust device if requested
        if (input.TrustDevice)
        {
            var deviceFingerprint = DeviceFingerprintHelper.GenerateFingerprint(httpContext);
            var deviceName = DeviceFingerprintHelper.GetDeviceName(httpContext);
            await twoFactorService.AddTrustedDeviceAsync(input.UserId, deviceFingerprint, deviceName);
        }

        // Get user roles
        var userRoles = await userRoleRepository.GetQueryable()
            .Where(ur => ur.UserId == user.Id)
            .Join(roleRepository.GetQueryable(),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToArrayAsync();

        // Generate tokens
        var token = jwtService.GenerateToken(
            user.Id,
            user.UserName,
            user.EmailAddress,
            user.TenantId,
            userRoles
        );

        var refreshToken = jwtService.GenerateRefreshToken();

        // Update last login time
        user.LastLoginTime = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        // Log successful login
        await auditRepository.InsertAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = user.TenantId,
            ExecutionTime = DateTime.UtcNow,
            ServiceName = "AuthEndpoints",
            MethodName = "VerifyOtpAsync",
            ClientIpAddress = auditContext.ClientIpAddress,
            BrowserInfo = auditContext.BrowserInfo,
            CustomData = input.TrustDevice 
                ? "User logged in successfully with 2FA - device trusted"
                : "User logged in successfully with 2FA"
        });

        return Results.Ok(new
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresIn = 28800
        });
    }

    private static async Task<IResult> ResendOtpAsync(
        ResendOtpDto input,
        IRepository<User> userRepository,
        IRepository<AuditLog> auditRepository,
        ITwoFactorService twoFactorService,
        IEmailSender emailSender,
        IAuditContext auditContext)
    {
        // Get user
        var user = await userRepository.GetAsync(input.UserId);
        if (user == null)
            return Results.Unauthorized();

        // Check rate limiting
        var canRequestOtp = await twoFactorService.CanRequestOtpAsync(input.UserId);
        if (!canRequestOtp)
        {
            return Results.Problem(
                "Too many OTP requests. Please try again later.",
                statusCode: 429);
        }

        // Invalidate old OTP and generate new one
        await twoFactorService.InvalidateOtpAsync(input.UserId);
        var otpCode = await twoFactorService.GenerateOtpAsync(input.UserId);

        // Send email
        var emailBody = TwoFactorEmailTemplate.GenerateOtpEmail(otpCode, 5);
        await emailSender.SendAsync(user.EmailAddress, "Your Verification Code", emailBody, isBodyHtml: true);

        // Log OTP resend
        await auditRepository.InsertAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = user.TenantId,
            ExecutionTime = DateTime.UtcNow,
            ServiceName = "AuthEndpoints",
            MethodName = "ResendOtpAsync",
            ClientIpAddress = auditContext.ClientIpAddress,
            BrowserInfo = auditContext.BrowserInfo,
            CustomData = "OTP code resent"
        });

        return Results.Ok(new { message = "Verification code sent to your email" });
    }
}

public class LoginDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
