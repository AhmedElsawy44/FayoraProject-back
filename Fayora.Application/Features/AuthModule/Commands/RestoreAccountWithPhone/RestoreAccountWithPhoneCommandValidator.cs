using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone
{

    public class RestoreAccountWithPhoneCommandValidator : AbstractValidator<RestoreAccountWithPhoneCommand>
    {
        public RestoreAccountWithPhoneCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(6).WithMessage("Code must be 6 digits.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required.")
                .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

            RuleFor(x => x.FcmToken)
                .NotEmpty().WithMessage("FCM Token is required.");

            RuleFor(x => x.DeviceLanguage)
                .NotEmpty().WithMessage("Device Language is required.");
        }
    }
}
