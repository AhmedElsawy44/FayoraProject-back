using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.SendEmailCode;

public class SendEmailCodeCommandValidator : AbstractValidator<SendEmailCodeCommand>
{
    public SendEmailCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.Purpose)
        .IsInEnum().WithMessage("Invalid OTP purpose.");
    }
}
