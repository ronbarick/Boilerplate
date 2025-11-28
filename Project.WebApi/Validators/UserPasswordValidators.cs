using FluentValidation;
using Project.Application.Users.Dtos;

namespace Project.WebApi.Validators;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("User:EmailRequired")
            .EmailAddress().WithMessage("User:EmailInvalid");
    }
}

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User:UserIdRequired");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("User:TokenRequired");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("User:PasswordRequired")
            .MinimumLength(8).WithMessage("User:PasswordMinLength")
            .Matches(@"[A-Z]").WithMessage("User:PasswordUppercase")
            .Matches(@"[a-z]").WithMessage("User:PasswordLowercase")
            .Matches(@"[0-9]").WithMessage("User:PasswordDigit")
            .Matches(@"[\W_]").WithMessage("User:PasswordSpecialChar");
    }
}

public class SetPasswordDtoValidator : AbstractValidator<SetPasswordDto>
{
    public SetPasswordDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User:UserIdRequired");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("User:TokenRequired");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("User:PasswordRequired")
            .MinimumLength(8).WithMessage("User:PasswordMinLength")
            .Matches(@"[A-Z]").WithMessage("User:PasswordUppercase")
            .Matches(@"[a-z]").WithMessage("User:PasswordLowercase")
            .Matches(@"[0-9]").WithMessage("User:PasswordDigit")
            .Matches(@"[\W_]").WithMessage("User:PasswordSpecialChar");
    }
}

public class ConfirmEmailDtoValidator : AbstractValidator<ConfirmEmailDto>
{
    public ConfirmEmailDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User:UserIdRequired");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("User:TokenRequired");
    }
}
