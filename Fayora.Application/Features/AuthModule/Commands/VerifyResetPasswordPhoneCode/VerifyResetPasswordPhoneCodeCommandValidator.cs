using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordPhoneCode
{
    public class VerifyResetPasswordPhoneCodeCommandValidator : AbstractValidator<VerifyResetPasswordPhoneCodeCommand>
    {
        public VerifyResetPasswordPhoneCodeCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

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
