using FluentValidation;
using Project.Application.Roles.Dtos;

namespace Project.Application.Roles.Validators;

public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role:NameRequired")
            .MinimumLength(2).WithMessage("Role:NameMinLength")
            .MaximumLength(50).WithMessage("Role:NameMaxLength")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Role:NameFormat");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Role:DisplayNameMaxLength");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Role:DescriptionMaxLength");
    }
}

public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role:NameRequired")
            .MinimumLength(2).WithMessage("Role:NameMinLength")
            .MaximumLength(50).WithMessage("Role:NameMaxLength")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Role:NameFormat");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Role:DisplayNameMaxLength");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Role:DescriptionMaxLength");
    }
}
