using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteEmailAccount;

public class VerifyDeleteEmailAccountCommandValidator : AbstractValidator<VerifyDeleteEmailAccountCommand>
{
    public VerifyDeleteEmailAccountCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be exactly 6 characters.")
            .Matches(@"^[0-9]{6}$").WithMessage("Verification code must contain only digits.");
    }
}
