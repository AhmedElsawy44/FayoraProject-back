using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.ResetPasswordPhone
{
    public class ResetPasswordPhoneCommandValidator : AbstractValidator<ResetPasswordPhoneCommand>
    {
        public ResetPasswordPhoneCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

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
