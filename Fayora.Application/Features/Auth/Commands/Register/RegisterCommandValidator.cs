using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(3, 50).WithMessage("First name must be between 3 and 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(3, 50).WithMessage("Last name must be between 3 and 50 characters.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Either email or phone number is required.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.SimCountryIsoCode)
            .NotEmpty()
            .WithMessage("SimCountryIsoCode is required when a phone number is provided.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.SimCountryIsoCode)
            .Length(2).WithMessage("SimCountryIsoCode must be exactly 2 characters (ISO code).")
            .When(x => !string.IsNullOrWhiteSpace(x.SimCountryIsoCode));

        RuleFor(x => x.DeviceLanguage)
            .NotEmpty().WithMessage("Device language is required.")
            .Length(2).WithMessage("Device language should be a 2-letter ISO code (e.g., 'en', 'ar').");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required.")
            .MaximumLength(100).WithMessage("Time zone name is too long.");

        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM token is required.")
            .MaximumLength(255).WithMessage("FCM token is too long.");
    }
}