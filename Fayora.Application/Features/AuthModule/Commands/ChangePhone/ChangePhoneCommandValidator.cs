using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.ChangePhone
{
    public class ChangePhoneCommandValidator : AbstractValidator<ChangePhoneCommand>
    {
        public ChangePhoneCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                 .NotEmpty().WithMessage("Phone number is required.")
                 .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                 .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(256).WithMessage("Password must not exceed 256 characters.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

            RuleFor(x => x.DeliveryMethod)
                .IsInEnum().WithMessage("Invalid delivery method.");
        }
    }
}
