using FluentValidation;
using Project.Application.Tenants.Dtos;

namespace Project.Application.Tenants.Validators;

public class CreateTenantDtoValidator : AbstractValidator<CreateTenantDto>
{
    public CreateTenantDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tenant:NameRequired")
            .MinimumLength(2).WithMessage("Tenant:NameMinLength")
            .MaximumLength(100).WithMessage("Tenant:NameMaxLength");

        RuleFor(x => x.TenancyName)
            .NotEmpty().WithMessage("Tenant:TenancyNameRequired")
            .MinimumLength(2).WithMessage("Tenant:TenancyNameMinLength")
            .MaximumLength(64).WithMessage("Tenant:TenancyNameMaxLength")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Tenant:TenancyNameFormat");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("User:PasswordRequired")
            .MinimumLength(8).WithMessage("User:PasswordMinLength")
            .MaximumLength(128).WithMessage("User:PasswordMaxLength")
            .Matches("[A-Z]").WithMessage("User:PasswordUppercase")
            .Matches("[a-z]").WithMessage("User:PasswordLowercase")
            .Matches("[0-9]").WithMessage("User:PasswordDigit")
            .Matches("[^a-zA-Z0-9]").WithMessage("User:PasswordSpecialChar");
    }
}
