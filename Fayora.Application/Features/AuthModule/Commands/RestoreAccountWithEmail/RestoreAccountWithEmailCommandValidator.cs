using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccountWithEmail
{
    public class RestoreAccountWithEmailCommandValidator : AbstractValidator<RestoreAccountWithEmailCommand>
    {
        public RestoreAccountWithEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(6).WithMessage("Code must be 6 digits.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

            RuleFor(x => x.FcmToken)
                .NotEmpty().WithMessage("FCM Token is required.");

            RuleFor(x => x.DeviceLanguage)
                .NotEmpty().WithMessage("Device Language is required.");
        }

    }
}

