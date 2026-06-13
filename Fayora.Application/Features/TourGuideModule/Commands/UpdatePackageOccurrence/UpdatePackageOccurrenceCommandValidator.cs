using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdatePackageOccurrence;

public class UpdatePackageOccurrenceCommandValidator : AbstractValidator<UpdatePackageOccurrenceCommand>
{
    public UpdatePackageOccurrenceCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("Package ID is required.");

        RuleFor(x => x.OccurrenceId)
            .NotEmpty().WithMessage("Occurrence ID is required.");

        RuleFor(x => x.NewDate)
            .NotEmpty().WithMessage("New date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Occurrence date must be today or in the future.");

        RuleFor(x => x.NewAvailableSeats)
            .GreaterThan(0).WithMessage("Available seats must be greater than zero.");
    }
}
