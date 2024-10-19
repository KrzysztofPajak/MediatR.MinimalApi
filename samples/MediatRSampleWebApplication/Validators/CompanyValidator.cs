using FluentValidation;
using static MediatRSampleWebApplication.Commands.Roles.CreateCompany;

namespace MediatRSampleWebApplication.Validators;
public class CompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CompanyValidator()
    {
        RuleFor(company => company.Company.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(10).WithMessage("Name cannot be longer than 10 characters.");
    }
}
