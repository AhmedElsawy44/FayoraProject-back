using Fayora.Application.Features.Auth.Commands.ResendRegisterOtp;
using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.ResendOtp;

public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.OtpPurpose)
            .IsInEnum().WithMessage("Invalid OTP purpose.");

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

    private bool HaveExactlyOneIdentifier(ResendOtpCommand command)
    {
        bool hasEmail = !string.IsNullOrWhiteSpace(command.Email);
        bool hasPhone = !string.IsNullOrWhiteSpace(command.PhoneNumber);

        return hasEmail ^ hasPhone;
    }
}