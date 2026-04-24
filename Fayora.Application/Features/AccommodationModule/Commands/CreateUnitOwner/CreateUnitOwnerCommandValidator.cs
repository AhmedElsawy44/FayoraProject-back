using FluentValidation;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;

public class CreateUnitOwnerCommandValidator : AbstractValidator<CreateUnitOwnerCommand>
{
    public CreateUnitOwnerCommandValidator()
    {
        RuleFor(c => c.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.OwnerType)
            .IsInEnum().WithMessage("Invalid owner type specified.");

        RuleFor(c => c.CommercialName)
            .NotEmpty().WithMessage("Commercial name is required.")
            .MaximumLength(100).WithMessage("Commercial name must not exceed 100 characters.");
    }
}