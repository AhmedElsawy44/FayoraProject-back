using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public class LoginWithEmailCommandValidator : AbstractValidator<LoginWithEmailCommand>
{
    public LoginWithEmailCommandValidator()
    {
        RuleFor(x => x.Email)
       .NotEmpty().WithMessage("Email is required.")
       .EmailAddress().WithMessage("Invalid email format.");

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
            .NotEmpty().WithMessage("Device language is required.");

    }
}
