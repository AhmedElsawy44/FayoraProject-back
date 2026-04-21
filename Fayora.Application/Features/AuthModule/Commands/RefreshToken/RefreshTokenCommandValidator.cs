using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.RefreshToken;


public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(20).WithMessage("Refresh token is too short.")
            .MaximumLength(2048).WithMessage("Refresh token is too long.")
            .Matches(@"^[A-Za-z0-9\-\._~\+\/]+=*$").WithMessage("Refresh token contains invalid characters.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");
    }
}
