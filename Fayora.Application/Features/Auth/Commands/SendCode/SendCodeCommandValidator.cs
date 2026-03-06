using Fayora.Application.Common.Validations;
using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.SendCode;

public class SendCodeCommandValidator : AbstractValidator<SendCodeCommand>
{
    public SendCodeCommandValidator()
    {

        RuleFor(x => x.OtpPurpose)
            .IsInEnum().WithMessage("Invalid OTP purpose.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x)
            .MustHaveExactlyOneIdentifier();
    }
}