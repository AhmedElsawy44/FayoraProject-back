using Fayora.Domain.Enums.IdentityModule;
using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.ResetPasswordEmail;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
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

        RuleFor(x => x.ResetToken)
        .NotEmpty().WithMessage("Reset token is required.")
        .Length(32, 128).WithMessage("Reset token must be between 32 and 128 characters.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(256).WithMessage("Password must not exceed 256 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid delivery method.");
    }
}
