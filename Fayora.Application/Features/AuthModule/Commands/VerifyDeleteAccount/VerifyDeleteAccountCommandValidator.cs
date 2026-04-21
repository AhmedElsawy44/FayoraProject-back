using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteAccount;

public class VerifyDeleteAccountCommandValidator : AbstractValidator<VerifyDeleteAccountCommand>
{
    public VerifyDeleteAccountCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be exactly 6 characters.")
            .Matches(@"^[0-9]{6}$").WithMessage("Verification code must contain only digits.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid verification type.");
    }
}
