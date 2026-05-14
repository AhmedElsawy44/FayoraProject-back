using FluentValidation;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitsByType
{
    public class GetUnitsByTypeQueryValidator : AbstractValidator<GetUnitsByTypeQuery>
    {
        public GetUnitsByTypeQueryValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid housing type. Valid values are: Apartment, Villa, Hotel.");
        }
    }
}
