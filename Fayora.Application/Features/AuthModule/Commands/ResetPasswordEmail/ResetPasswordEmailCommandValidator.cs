using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.ResetPasswordEmail
{
    public class ResetPasswordEmailCommandValidator : AbstractValidator<ResetPasswordEmailCommand>
    {
        public ResetPasswordEmailCommandValidator()
        {
            RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.ResetToken)
            .NotEmpty().WithMessage("Reset token is required.")
            .Length(32, 128).WithMessage("Reset token must be between 32 and 128 characters.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");
        }
    }
}
