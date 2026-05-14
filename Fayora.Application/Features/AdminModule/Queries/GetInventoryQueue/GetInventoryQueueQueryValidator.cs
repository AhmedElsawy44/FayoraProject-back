using FluentValidation;

namespace Fayora.Application.Features.AdminModule.Queries.GetInventoryQueue;

public class GetInventoryQueueQueryValidator : AbstractValidator<GetInventoryQueueQuery>
{
    public GetInventoryQueueQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be greater than or equal to 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100.");

        RuleFor(x => x.TypeFilter)
            .IsInEnum()
            .When(x => x.TypeFilter.HasValue)
            .WithMessage("Invalid type filter value.");
    }
}

