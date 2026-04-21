using FluentValidation;

namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest;

public class SubmitVerificationRequestCommandValidator : AbstractValidator<SubmitVerificationRequestCommand>
{
    public SubmitVerificationRequestCommandValidator()
    {
        RuleFor(x => x.RequestType)
            .IsInEnum().WithMessage("Invalid request type.");

        RuleFor(x => x.Documents)
            .NotNull().WithMessage("Documents are required.")
            .NotEmpty().WithMessage("At least one document is required.");

        RuleForEach(x => x.Documents).ChildRules(d =>
        {
            d.RuleFor(x => x.DocumentType)
                .IsInEnum().WithMessage("Invalid document type.");

            d.RuleFor(x => x.FileUrl)
                .NotEmpty().WithMessage("File URL is required.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("File URL must be a valid absolute URL.")
                .MaximumLength(2048).WithMessage("File URL must not exceed 2048 characters.");
        });
    }
}
