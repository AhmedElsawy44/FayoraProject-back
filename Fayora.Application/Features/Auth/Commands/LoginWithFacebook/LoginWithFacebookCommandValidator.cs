using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.LoginWithFacebook
{
    public class LoginWithFacebookCommandValidator : AbstractValidator<LoginWithFacebookCommand>
    {
        public LoginWithFacebookCommandValidator()
        {
            RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Facebook Access Token is required.")
            .MinimumLength(50).WithMessage("Access Token is too short to be a valid Facebook token.");

            RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

            RuleFor(x => x.FcmToken)
                .NotEmpty().WithMessage("FCM Token is required.")
                .MaximumLength(500).WithMessage("FCM Token must not exceed 500 characters.");

            RuleFor(x => x.DeviceLanguage)
                .NotEmpty().WithMessage("Device language is required.")
                .Length(2).WithMessage("Device language should be a 2-letter ISO code (e.g., 'en', 'ar').");
        }
    }
}
