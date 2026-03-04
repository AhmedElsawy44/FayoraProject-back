using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordOtp;

public class VerifyResetPasswordOtpCommandValidator : AbstractValidator<VerifyResetPasswordOtpCommand>
{
    public VerifyResetPasswordOtpCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be 6 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x)
            .Must(HaveExactlyOneIdentifier)
            .WithMessage("You must provide either Email or Phone Number, not both.")
            .WithName("Identifier");
    }

    private bool HaveExactlyOneIdentifier(VerifyResetPasswordOtpCommand command)
    {
        bool hasEmail = !string.IsNullOrWhiteSpace(command.Email);
        bool hasPhone = !string.IsNullOrWhiteSpace(command.PhoneNumber);

        return hasEmail ^ hasPhone;
    }
}
