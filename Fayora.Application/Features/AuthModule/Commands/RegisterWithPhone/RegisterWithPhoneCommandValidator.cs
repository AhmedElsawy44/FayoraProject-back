using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.RegisterWithPhone
{
    public class RegisterWithPhoneCommandValidator : AbstractValidator<RegisterWithPhoneCommand>
    {
        public RegisterWithPhoneCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");
        }
    }
}
