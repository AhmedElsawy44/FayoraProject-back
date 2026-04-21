using FluentValidation;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetUnitById;

public class GetUnitByIdQueryValidator : AbstractValidator<GetUnitByIdQuery>
{
    public GetUnitByIdQueryValidator()
    {
        RuleFor(x => x.UnitId)
            .NotEmpty()
            .WithMessage("UnitId cannot be empty.");
    }
}