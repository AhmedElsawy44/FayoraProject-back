using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllLocations
{
    public class GetLocationsQueryValidator : AbstractValidator<GetAllLocationsQuery>
    {
        public GetLocationsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

            RuleFor(x => x.MinRating)
                .InclusiveBetween(0, 5).When(x => x.MinRating.HasValue)
                .WithMessage("Rating must be between 0 and 5.");
        }
    }
}
