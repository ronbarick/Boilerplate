using FluentValidation;
using Project.WebApi.Endpoints;

namespace Project.WebApi.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User:UsernameRequired")
            .MinimumLength(3).WithMessage("User:UsernameMinLength")
            .MaximumLength(50).WithMessage("User:UsernameMaxLength");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("User:PasswordRequired")
            .MinimumLength(8).WithMessage("User:PasswordMinLength");
    }
}
