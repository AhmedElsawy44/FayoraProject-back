using Fayora.Domain.Enums.IdentityModule;
using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordCode;

public class VerifyResetPasswordCodeCommandValidator : AbstractValidator<VerifyResetPasswordCodeCommand>
{
    public VerifyResetPasswordCodeCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required.");

        When(x => x.Type == CodeDeliveryMethod.Email, () =>
        {
            RuleFor(x => x.Value)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");
        });

        When(x => x.Type is CodeDeliveryMethod.Sms or CodeDeliveryMethod.WhatsApp, () =>
        {
            RuleFor(x => x.Value)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
        });

        RuleFor(x => x.Code)
        .NotEmpty().WithMessage("Verification code is required.")
        .Length(6).WithMessage("Verification code must be exactly 6 characters.")
        .Matches(@"^[0-9]{6}$").WithMessage("Verification code must contain only digits.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid delivery method.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");
    }
}
