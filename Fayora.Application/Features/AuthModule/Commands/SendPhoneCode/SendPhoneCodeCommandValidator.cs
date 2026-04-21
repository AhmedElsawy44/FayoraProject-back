using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.SendPhoneCode;

public class SendPhoneCodeCommandValidator : AbstractValidator<SendPhoneCodeCommand>
{
    public SendPhoneCodeCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.Purpose)
            .IsInEnum().WithMessage("Invalid OTP purpose.");

        RuleFor(x => x.DeliveryMethod)
            .IsInEnum().WithMessage("Invalid OTP delivery method.");
    }
}
