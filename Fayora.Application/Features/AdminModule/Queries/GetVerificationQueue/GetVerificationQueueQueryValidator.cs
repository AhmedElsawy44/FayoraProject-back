using FluentValidation;

namespace Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;

public class GetVerificationQueueQueryValidator : AbstractValidator<GetVerificationQueueQuery>
{
    public GetVerificationQueueQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid partner type filter.");
    }
}
