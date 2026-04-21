using Fayora.Domain.Enums.IdentityModule;
using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;

public class RestoreAccountCommandValidator : AbstractValidator<RestoreAccountCommand>
{
    public RestoreAccountCommandValidator()
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
            .NotEmpty().WithMessage("Code is required.")
            .Length(6).WithMessage("Code must be 6 digits.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid delivery method.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM Token is required.")
            .MaximumLength(500).WithMessage("FCM Token must not exceed 500 characters.");

        RuleFor(x => x.DeviceLanguage)
            .IsInEnum().WithMessage("Invalid device language.");
    }

}

