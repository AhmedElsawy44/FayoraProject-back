using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public class ConfirmChangePhoneCommandValidator : AbstractValidator<ConfirmChangePhoneCommand>
{
    public ConfirmChangePhoneCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.NewPhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Invalid phone number format. Use international format (e.g., +2010...)");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be 6 digits.");
    }
}