using FluentValidation;

namespace Fayora.Application.Features.ReviewModule.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("BookingId is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1m, 5m).WithMessage("Rating must be between 1 and 5.")
            .Must(x => x % 0.5m == 0).WithMessage("Rating must be in increments of 0.5.");

        RuleFor(x => x.Comment)
            .MinimumLength(10).WithMessage("Comment must be at least 10 characters.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
            .When(x => x.Comment != null);
    }
}
