using Fayora.Application.Features.Auth.Commands.ResendOtp;
using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {

        RuleFor(x => x.Email)
           .EmailAddress().WithMessage("Invalid email format.")
           .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x)
            .Must(HaveExactlyOneIdentifier)
            .WithMessage("You must provide either Email or Phone Number, not both.")
            .WithName("Identifier");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.SimCountryIsoCode)
            .Length(2).WithMessage("SimCountryIsoCode must be exactly 2 characters (ISO code).");


        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.DeviceLanguage)
            .NotEmpty().WithMessage("Device language is required.")
            .Length(2).WithMessage("Device language should be a 2-letter ISO code (e.g., 'en', 'ar').");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required.")
            .MaximumLength(100).WithMessage("Time zone name is too long.");
    }

    private bool HaveExactlyOneIdentifier(RegisterCommand command)
    {
        bool hasEmail = !string.IsNullOrWhiteSpace(command.Email);
        bool hasPhone = !string.IsNullOrWhiteSpace(command.PhoneNumber);

        return hasEmail ^ hasPhone;
    }
}