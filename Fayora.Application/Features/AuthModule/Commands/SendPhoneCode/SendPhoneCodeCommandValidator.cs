using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.SendPhoneCode;

public class SendPhoneCodeCommandValidator : AbstractValidator<SendPhoneCodeCommand>
{
    public SendPhoneCodeCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.Purpose)
        .IsInEnum().WithMessage("Invalid OTP purpose."); RuleFor(x => x.PhoneNumber);

        RuleFor(x => x.DeliveryMethod)
            .NotEmpty().WithMessage("Delivery method is required.")
            .IsInEnum().WithMessage("Invalid OTP delivery method.");
    }
}
