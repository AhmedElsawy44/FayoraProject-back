using FluentValidation;
using Fayora.Application.Features.TourCompanyModule.CreateTourCompany;

namespace Fayora.Application.Features.TourCompanyModule.Commands.CreateTourCompany
{

    public class CreateTourCompanyCommandValidator : AbstractValidator<CreateTourCompanyCommand>
    {
        public CreateTourCompanyCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.CommercialRegisterNumber)
                .NotEmpty().WithMessage("Commercial register number is required.")
                .MaximumLength(100).WithMessage("Commercial register number cannot exceed 100 characters.");

            RuleFor(x => x.TaxRegistrationNumber)
                .NotEmpty().WithMessage("Tax registration number is required.")
                .MaximumLength(100).WithMessage("Tax registration number cannot exceed 100 characters.");

            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage("Currency code is required.")
                .Length(3).WithMessage("Currency code must be exactly 3 characters.");

            RuleFor(x => x.LogoUrl)
                .MaximumLength(500).WithMessage("Logo URL cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
        }
    }
}
