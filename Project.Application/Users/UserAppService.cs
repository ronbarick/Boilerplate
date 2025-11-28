
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Application.Common.Dtos;
using Project.Core.Attributes;
using Project.Core.Constants;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Infrastructure.Extensions;
using Project.Application.Services;
using Project.Application.Users.Dtos;
using Project.Core.Localization;
using Project.Core.BackgroundJobs;
using Project.Application.BackgroundJobs;
using Project.Core.Exceptions;

namespace Project.Application.Users;

public class UserAppService : AppServiceBase, IUserAppService
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserManager _userManager;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ILogger<UserAppService> _logger;

    public UserAppService(
        IRepository<User> userRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        ILocalizationManager localizationManager,
        IUserManager userManager,
        IBackgroundJobManager backgroundJobManager,
        ILogger<UserAppService> logger)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userManager = userManager;
        _backgroundJobManager = backgroundJobManager;
        _logger = logger;
    }

    [RequiresPermission(PermissionNames.Pages_Users)]
    public async Task<PagedResultDto<UserDto>> GetListAsync(GetUsersInput input)
    {
        input.Normalize();

        var query = _userRepository.GetQueryable();

        // Apply filter/search using WhereIf
        var filter = input.Filter?.ToLower();
        query = query.WhereIf(
            !string.IsNullOrWhiteSpace(filter),
            u => u.Name.ToLower().Contains(filter!) ||
                 u.EmailAddress.ToLower().Contains(filter!) ||
                 (u.UserName != null && u.UserName.ToLower().Contains(filter!)));

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderByDynamic(input.Sorting ?? "Name ASC")
            .PageBy(input.SkipCount, input.MaxResultCount)
            .ToListAsync();

        return new PagedResultDto<UserDto>
        {
            Items = _mapper.Map<List<UserDto>>(users),
            TotalCount = totalCount
        };
    }

    [RequiresPermission(PermissionNames.Pages_Users, PermissionNames.Pages_Users_Create)]
    public async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        _logger.LogInformation("Creating user: {UserName}, {Email}", input.UserName, input.EmailAddress);

        try
        {
            User user;
            string? plainToken = null;

            if (!string.IsNullOrWhiteSpace(input.Password))
            {
                // Scenario A: Admin creates user WITH password
                user = await _userManager.CreateUserWithPasswordAsync(
                    input.UserName,
                    input.Name,
                    input.Surname,
                    input.EmailAddress,
                    input.Password,
                    input.PhoneNumber);

                // Queue welcome email if requested
                if (input.SendWelcomeEmail)
                {
                    _backgroundJobManager.Enqueue<SendWelcomeEmailJob>(job => job.ExecuteAsync(user.Id));
                    _logger.LogInformation("Queued welcome email for user {UserId}", user.Id);
                }
            }
            else
            {
                // Scenario B: Admin creates user WITHOUT password
                // Returns tuple to avoid duplicate DB call
                (user, plainToken) = await _userManager.CreateUserWithoutPasswordAsync(
                    input.UserName,
                    input.Name,
                    input.Surname,
                    input.EmailAddress,
                    input.PhoneNumber);

                // Queue set password email
                _backgroundJobManager.Enqueue<SendSetPasswordEmailJob>(job => job.ExecuteAsync(user.Id, plainToken));
                _logger.LogInformation("Queued set-password email for user {UserId}", user.Id);
            }

            // Generate email confirmation token and send confirmation email
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            _backgroundJobManager.Enqueue<SendEmailConfirmationJob>(job => job.ExecuteAsync(user.Id, emailToken));
            _logger.LogInformation("User created successfully: {UserId}, {UserName}", user.Id, input.UserName);

            return _mapper.Map<UserDto>(user);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            _logger.LogWarning(ex, "Duplicate user creation attempt: {UserName}, {Email}", input.UserName, input.EmailAddress);
            throw new WarningException(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user: {UserName}, {Email}", input.UserName, input.EmailAddress);
            throw;
        }
    }

    [RequiresPermission(PermissionNames.Pages_Users)]
    public async Task<UserDto> GetAsync(Guid id)
    {
        var user = await _userRepository.GetAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    [RequiresPermission(PermissionNames.Pages_Users, PermissionNames.Pages_Users_Edit)]
    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input)
    {
        var user = await _userRepository.GetAsync(id);

        user.Name = input.Name;
        user.Surname = input.Surname;
        user.EmailAddress = input.EmailAddress;
        user.PhoneNumber = input.PhoneNumber;
        user.IsActive = input.IsActive;

        if (!string.IsNullOrEmpty(input.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);
        }

        await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    [RequiresPermission(PermissionNames.Pages_Users, PermissionNames.Pages_Users_Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }

    // Password management operations
    [Project.Core.Attributes.RemoteService(false)]
    public async Task ForgotPasswordAsync(ForgotPasswordDto input)
    {
        _logger.LogInformation("Forgot password request for email: {Email}", input.EmailAddress);

        var user = await _userRepository.GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.EmailAddress == input.EmailAddress);

        if (user == null)
        {
            // Don't reveal if email exists for security reasons
            _logger.LogWarning("Forgot password request for non-existent email: {Email}", input.EmailAddress);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
        _backgroundJobManager.Enqueue<SendResetPasswordEmailJob>(job => job.ExecuteAsync(user.Id, token));
        _logger.LogInformation("Queued password reset email for user {UserId}", user.Id);
    }

    [Project.Core.Attributes.RemoteService(false)]
    public async Task ResetPasswordAsync(ResetPasswordDto input)
    {
        _logger.LogInformation("Resetting password for user {UserId}", input.UserId);
        await _userManager.ResetPasswordAsync(input.UserId, input.Token, input.NewPassword);
        _logger.LogInformation("Password reset completed for user {UserId}", input.UserId);
    }

    [Project.Core.Attributes.RemoteService(false)]
    public async Task SetPasswordAsync(SetPasswordDto input)
    {
        _logger.LogInformation("Setting password for user {UserId}", input.UserId);

        if (!await _userManager.ValidatePasswordResetTokenAsync(input.UserId, input.Token))
        {
            _logger.LogWarning("Set password failed: Invalid token for user {UserId}", input.UserId);
            throw new InvalidOperationException("Invalid or expired token");
        }

        await _userManager.SetPasswordAsync(input.UserId, input.NewPassword);
        _logger.LogInformation("Password set successfully for user {UserId}", input.UserId);
    }

    // Email confirmation operations
    [Project.Core.Attributes.RemoteService(false)]
    public async Task ConfirmEmailAsync(ConfirmEmailDto input)
    {
        _logger.LogInformation("Confirming email for user {UserId}", input.UserId);

        if (!await _userManager.ValidateEmailConfirmationTokenAsync(input.UserId, input.Token))
        {
            _logger.LogWarning("Email confirmation failed: Invalid token for user {UserId}", input.UserId);
            throw new InvalidOperationException("Invalid or expired token");
        }

        var user = await _userRepository.GetAsync(input.UserId);
        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.TokenExpiration = null;

        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Email confirmed successfully for user {UserId}", input.UserId);
    }

    [Project.Core.Attributes.RemoteService(false)]
    public async Task ResendConfirmationEmailAsync(Guid userId)
    {
        _logger.LogInformation("Resending confirmation email for user {UserId}", userId);

        var user = await _userRepository.GetAsync(userId);

        if (user.IsEmailConfirmed)
        {
            _logger.LogWarning("Resend confirmation failed: Email already confirmed for user {UserId}", userId);
            throw new InvalidOperationException("Email is already confirmed");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
        _backgroundJobManager.Enqueue<SendEmailConfirmationJob>(job => job.ExecuteAsync(userId, token));
        _logger.LogInformation("Queued confirmation email for user {UserId}", userId);
    }
}
