using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreatePackageOccurrences;


public class CreatePackageOccurrencesCommandValidator : AbstractValidator<CreatePackageOccurrencesCommand>
{
    public CreatePackageOccurrencesCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty();

        RuleFor(x => x.Occurrences)
            .NotEmpty().WithMessage("At least one occurrence is required.")
            .Must(list => list.Count <= 365)
            .WithMessage("Cannot add more than 365 occurrences at once.");

        RuleForEach(x => x.Occurrences).ChildRules(occurrence =>
        {
            occurrence.RuleFor(x => x.Date)
                .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date must be in the future.");

            occurrence.RuleFor(x => x.AvailableSeats)
                .GreaterThan(0)
                .WithMessage("Available seats must be greater than 0.");
        });

        RuleFor(x => x.Occurrences)
            .Must(list => list.Select(x => x.Date).Distinct().Count() == list.Count)
            .WithMessage("Duplicate dates are not allowed.");
    }
}
