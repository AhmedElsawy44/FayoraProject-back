using Fayora.Domain.Enums.SharedModule;
using FluentValidation;

namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest;

public class ReviewVerificationRequestCommandValidator : AbstractValidator<ReviewVerificationRequestCommand>
{
    public ReviewVerificationRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("RequestId must be greater than 0.");

        RuleFor(x => x.NewStatus)
            .Must(s => s == RequestStatus.Approved || s == RequestStatus.Rejected)
            .WithMessage("Status must be Approved or Rejected.");

        When(x => x.NewStatus == RequestStatus.Rejected, () =>
        {
            RuleFor(x => x.AdminComment)
                .NotEmpty().WithMessage("Admin comment is required when rejecting.")
                .MaximumLength(1000).WithMessage("Admin comment must not exceed 1000 characters.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.AdminComment), () =>
        {
            RuleFor(x => x.AdminComment!)
                .MaximumLength(1000).WithMessage("Admin comment must not exceed 1000 characters.");
        });
    }
}
