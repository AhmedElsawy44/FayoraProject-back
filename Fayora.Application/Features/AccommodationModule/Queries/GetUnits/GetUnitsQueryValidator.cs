using FluentValidation;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnits
{
    public class GetUnitsQueryValidator : AbstractValidator<GetUnitsQuery>
    {
        public GetUnitsQueryValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum()
                .When(x => x.Type.HasValue)
                .WithMessage("Invalid housing type. Valid values are: Apartment, Villa, Hotel.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm))
                .WithMessage("Search term must not exceed 100 characters.");
        }
    }
}
