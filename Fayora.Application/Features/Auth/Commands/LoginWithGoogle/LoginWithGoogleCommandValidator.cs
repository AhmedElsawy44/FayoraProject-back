using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.LoginWithGoogle;

public class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID Token is required.")
            .Must(x => x.Split('.').Length == 3)
            .WithMessage("Invalid ID Token format. It must be a valid JWT.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM Token is required.")
            .MaximumLength(500).WithMessage("FCM Token must not exceed 500 characters.");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required.")
            .MaximumLength(50).WithMessage("Time zone must not exceed 50 characters (e.g., 'Africa/Cairo').");

        RuleFor(x => x.DeviceLanguage)
            .NotEmpty().WithMessage("Device language is required.")
            .Length(2).WithMessage("Device language should be a 2-letter ISO code (e.g., 'en', 'ar').");

        RuleFor(x => x.SimCountryIsoCode)
            .MaximumLength(5).WithMessage("SIM Country ISO code is invalid.")
            .When(x => !string.IsNullOrEmpty(x.SimCountryIsoCode));

    }
}