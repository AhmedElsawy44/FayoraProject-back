using Fayora.Domain.Enums.AccommodationModule;
using FluentValidation;

namespace Fayora.Application.Features.Accommodation.Commands.CreateHousingOwner;

public class CreateHousingOwnerCommandValidator : AbstractValidator<CreateHousingOwnerCommand>
{
    public CreateHousingOwnerCommandValidator()
    {
        RuleFor(x => x.OwnerType)
            .IsInEnum().WithMessage("Invalid owner type specified.");

        RuleFor(x => x.NationalIdUrl)
            .NotEmpty().WithMessage("National ID URL is required.");

        When(x => x.OwnerType == UnitOwnerType.Commercial, () =>
        {
            RuleFor(x => x.CommercialName)
                .NotEmpty().WithMessage("Commercial name is required for commercial owners.")
                .MaximumLength(100).WithMessage("Commercial name must not exceed 100 characters.");

            RuleFor(x => x.TaxRegistrationNumber)
                .NotEmpty().WithMessage("Tax registration number is required for commercial owners.")
                .Matches(@"^\d{3}-?\d{3}-?\d{3}$")
                .WithMessage("Invalid tax registration number format. It must be 9 digits (e.g., 123-456-789).");
        });
    }
}