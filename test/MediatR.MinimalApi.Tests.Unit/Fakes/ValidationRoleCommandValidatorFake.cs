using FluentValidation;

namespace MediatR.MinimalApi.Tests.Unit.Fakes;

public class ValidationRoleCommandValidatorFake : AbstractValidator<ValidationRoleCommandFake>
{
    public ValidationRoleCommandValidatorFake()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(10).WithMessage("Name cannot exceed 10 characters");
    }
}
