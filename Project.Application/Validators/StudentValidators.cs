using FluentValidation;
using Project.Application.Students.Dtos;

namespace Project.Application.Validators;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Student:NameRequired")
            .MaximumLength(50).WithMessage("Student:NameMaxLength");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Student:NameRequired")
            .MaximumLength(50).WithMessage("Student:NameMaxLength");

        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Student:EmailRequired")
            .EmailAddress().WithMessage("Student:EmailInvalid")
            .MaximumLength(256).WithMessage("Student:EmailMaxLength");

        RuleFor(x => x.Grade)
            .InclusiveBetween(1, 12).WithMessage("Student:AgeRange");
    }
}

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Student:NameRequired")
            .MaximumLength(50).WithMessage("Student:NameMaxLength");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Student:NameRequired")
            .MaximumLength(50).WithMessage("Student:NameMaxLength");

        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Student:EmailRequired")
            .EmailAddress().WithMessage("Student:EmailInvalid")
            .MaximumLength(256).WithMessage("Student:EmailMaxLength");

        RuleFor(x => x.Grade)
            .InclusiveBetween(1, 12).WithMessage("Student:AgeRange");
    }
}
