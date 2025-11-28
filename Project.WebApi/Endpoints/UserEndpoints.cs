using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Project.Application.Users;
using Project.Application.Users.Dtos;
using System;
using System.Threading.Tasks;

namespace Project.WebApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        // Public endpoints for password management
        group.MapPost("/forgot-password", ForgotPasswordAsync)
            .AllowAnonymous()
            .WithName("ForgotPassword");

        group.MapPost("/reset-password", ResetPasswordAsync)
            .AllowAnonymous()
            .WithName("ResetPassword");

        group.MapPost("/set-password", SetPasswordAsync)
            .AllowAnonymous()
            .WithName("SetPassword");

        group.MapPost("/confirm-email", ConfirmEmailAsync)
            .AllowAnonymous()
            .WithName("ConfirmEmail");

        group.MapPost("/{userId:guid}/resend-confirmation", ResendConfirmationEmailAsync)
            .AllowAnonymous()
            .WithName("ResendConfirmationEmail");
    }

    private static async Task<IResult> ForgotPasswordAsync(
        ForgotPasswordDto input,
        IUserAppService userAppService)
    {
        await userAppService.ForgotPasswordAsync(input);
        return Results.Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    private static async Task<IResult> ResetPasswordAsync(
        ResetPasswordDto input,
        IUserAppService userAppService)
    {
        try
        {
            await userAppService.ResetPasswordAsync(input);
            return Results.Ok(new { message = "Password has been reset successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> SetPasswordAsync(
        SetPasswordDto input,
        IUserAppService userAppService)
    {
        try
        {
            await userAppService.SetPasswordAsync(input);
            return Results.Ok(new { message = "Password has been set successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> ConfirmEmailAsync(
        ConfirmEmailDto input,
        IUserAppService userAppService)
    {
        try
        {
            await userAppService.ConfirmEmailAsync(input);
            return Results.Ok(new { message = "Email has been confirmed successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> ResendConfirmationEmailAsync(
        Guid userId,
        IUserAppService userAppService)
    {
        try
        {
            await userAppService.ResendConfirmationEmailAsync(userId);
            return Results.Ok(new { message = "Confirmation email has been sent." });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}
