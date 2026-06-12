using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageOccurrenceDetails;

public class GetPackageOccurrenceDetailsQueryValidator : AbstractValidator<GetPackageOccurrenceDetailsQuery>
{
    public GetPackageOccurrenceDetailsQueryValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("Package ID is required.");

        RuleFor(x => x.OccurrenceId)
            .NotEmpty().WithMessage("Occurrence ID is required.");
    }
}
