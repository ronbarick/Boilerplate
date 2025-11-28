using FluentValidation;
using Project.Application.Users.Dtos;

namespace Project.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User:UsernameRequired")
            .MinimumLength(3).WithMessage("User:UsernameMinLength")
            .MaximumLength(50).WithMessage("User:UsernameMaxLength")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("User:UsernameFormat");

        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("User:EmailRequired")
            .EmailAddress().WithMessage("User:EmailInvalid")
            .MaximumLength(256).WithMessage("User:EmailMaxLength");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("User:PasswordRequired")
            .MinimumLength(8).WithMessage("User:PasswordMinLength")
            .MaximumLength(128).WithMessage("User:PasswordMaxLength")
            .Matches("[A-Z]").WithMessage("User:PasswordUppercase")
            .Matches("[a-z]").WithMessage("User:PasswordLowercase")
            .Matches("[0-9]").WithMessage("User:PasswordDigit")
            .Matches("[^a-zA-Z0-9]").WithMessage("User:PasswordSpecialChar");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User:NameRequired")
            .MaximumLength(50).WithMessage("User:NameMaxLength");

        RuleFor(x => x.Surname)
            .MaximumLength(50).WithMessage("User:SurnameMaxLength");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("User:PhoneMaxLength");
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("User:EmailRequired")
            .EmailAddress().WithMessage("User:EmailInvalid")
            .MaximumLength(256).WithMessage("User:EmailMaxLength");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User:NameRequired")
            .MaximumLength(50).WithMessage("User:NameMaxLength");

        RuleFor(x => x.Surname)
            .MaximumLength(50).WithMessage("User:SurnameMaxLength");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("User:PhoneMaxLength");
    }
}
