using Fayora.Domain.Enums.IdentityModule;
using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithSocial;

public class LoginWithAppleCommandValidator : AbstractValidator<LoginWithSocialCommand>
{
    public LoginWithAppleCommandValidator()
    {
        RuleFor(x => x.Token)
        .NotEmpty().WithMessage("Apple ID Token is required.")
        .MinimumLength(50).WithMessage("ID Token is too short to be a valid Apple token.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.")
            .When(x => x.IdentityProvider == IdentityProvider.Apple);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
            .When(x => x.IdentityProvider == IdentityProvider.Apple);

        RuleFor(x => x.DeviceId)
        .NotEmpty().WithMessage("Device ID is required.")
        .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM Token is required.")
            .MaximumLength(500).WithMessage("FCM Token must not exceed 500 characters.");

        RuleFor(x => x.DeviceLanguage)
            .IsInEnum()
            .WithMessage("Invalid device language.");


        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required.")
            .MaximumLength(50).WithMessage("Time zone must not exceed 50 characters (e.g., 'Africa/Cairo').");

        RuleFor(x => x.SimCountryIsoCode)
            .Length(2).WithMessage("SIM Country ISO code must be exactly 2 letters (e.g., 'EG', 'US').")
            .When(x => !string.IsNullOrWhiteSpace(x.SimCountryIsoCode));
    }
}
