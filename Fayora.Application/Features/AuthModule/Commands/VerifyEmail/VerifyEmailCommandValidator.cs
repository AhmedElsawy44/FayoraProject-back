using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyEmail;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be exactly 6 characters.")
            .Matches(@"^[0-9]{6}$").WithMessage("Verification code must contain only digits.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM Token is required.")
            .MaximumLength(500).WithMessage("FCM Token must not exceed 500 characters.");

        RuleFor(x => x.DeviceLanguage)
            .NotEmpty().WithMessage("Device language is required.")
            .Length(2).WithMessage("Device language should be a 2-letter ISO code (e.g., 'en', 'ar').");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required.")
            .MaximumLength(50).WithMessage("Time zone must not exceed 50 characters (e.g., 'Africa/Cairo').");

        RuleFor(x => x.SimCountryIsoCode)
            .Length(2).WithMessage("SIM Country ISO code must be exactly 2 letters (e.g., 'EG', 'US').")
            .When(x => !string.IsNullOrWhiteSpace(x.SimCountryIsoCode));

    }
}
