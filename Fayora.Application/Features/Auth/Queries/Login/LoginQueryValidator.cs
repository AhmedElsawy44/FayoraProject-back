using Fayora.Application.Common.Validations;
using FluentValidation;

namespace Fayora.Application.Features.Auth.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.Email)
           .EmailAddress().WithMessage("Invalid email format.")
           .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x).MustHaveExactlyOneIdentifier();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.");

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
