using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages
{
    public class GetMyPackagesQueryValidator : AbstractValidator<GetMyPackagesQuery>
    {
        public GetMyPackagesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Invalid status value.");
        }
    }
}
