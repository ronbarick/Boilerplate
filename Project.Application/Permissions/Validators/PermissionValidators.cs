using FluentValidation;
using Project.Application.Permissions.Dtos;

namespace Project.Application.Permissions.Validators;

public class UpdateRolePermissionsInputValidator : AbstractValidator<UpdateRolePermissionsInput>
{
    public UpdateRolePermissionsInputValidator()
    {
        RuleFor(x => x)
            .Must(x => x.RoleId.HasValue || !string.IsNullOrEmpty(x.RoleName))
            .WithMessage("Permission:RoleRequired");

        RuleFor(x => x.RoleName)
            .MaximumLength(50).WithMessage("Role:NameMaxLength")
            .When(x => !string.IsNullOrEmpty(x.RoleName));

        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("Permission:AtLeastOneRequired");

        RuleForEach(x => x.Permissions)
            .ChildRules(perm =>
            {
                perm.RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Permission:NameRequired");
            });
    }
}

public class UpdatePermissionsDtoValidator : AbstractValidator<UpdatePermissionsDto>
{
    public UpdatePermissionsDtoValidator()
    {
        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("Permission:AtLeastOneRequired");

        RuleForEach(x => x.Permissions)
            .ChildRules(perm =>
            {
                perm.RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Permission:NameRequired");
            });
    }
}

public class GetRolePermissionsInputValidator : AbstractValidator<GetRolePermissionsInput>
{
    public GetRolePermissionsInputValidator()
    {
        RuleFor(x => x)
            .Must(x => x.RoleId.HasValue || !string.IsNullOrEmpty(x.RoleName))
            .WithMessage("Permission:RoleRequired");

        RuleFor(x => x.RoleName)
            .MaximumLength(50).WithMessage("Role:NameMaxLength")
            .When(x => !string.IsNullOrEmpty(x.RoleName));
    }
}
