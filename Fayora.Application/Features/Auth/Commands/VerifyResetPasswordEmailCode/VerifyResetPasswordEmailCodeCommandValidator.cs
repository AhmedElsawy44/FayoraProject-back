using FluentValidation;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordEmailCode
{
    public class VerifyResetPasswordEmailCodeCommandValidator : AbstractValidator<VerifyResetPasswordEmailCodeCommand>
    {
        public VerifyResetPasswordEmailCodeCommandValidator()
        {
            RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Verification code must be exactly 6 characters.")
            .Matches(@"^[0-9]{6}$").WithMessage("Verification code must contain only digits.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");
        }
    }
}
