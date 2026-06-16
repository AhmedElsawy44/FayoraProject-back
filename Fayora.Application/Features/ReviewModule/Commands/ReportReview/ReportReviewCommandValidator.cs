using FluentValidation;

namespace Fayora.Application.Features.ReviewModule.Commands.ReportReview;

public class ReportReviewCommandValidator : AbstractValidator<ReportReviewCommand>
{
    public ReportReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("ReviewId is required.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Report reason is required.");

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(500).WithMessage("Additional notes must not exceed 500 characters.")
            .When(x => x.AdditionalNotes != null);
    }
}
