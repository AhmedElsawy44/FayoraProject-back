using FluentValidation;

namespace Fayora.Application.Features.ReviewModule.Queries.GetReviewsByTarget;

public class GetReviewsByTargetQueryValidator : AbstractValidator<GetReviewsByTargetQuery>
{
    public GetReviewsByTargetQueryValidator()
    {
        RuleFor(x => x.TargetId)
            .NotEmpty().WithMessage("TargetId is required.");

        RuleFor(x => x.TargetType)
            .NotEmpty().WithMessage("TargetType is required.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
    }
}
