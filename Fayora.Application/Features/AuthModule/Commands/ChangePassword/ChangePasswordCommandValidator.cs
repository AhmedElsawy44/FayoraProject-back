using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .MaximumLength(256).WithMessage("Password must not exceed 256 characters.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.")
            .When(x => !string.IsNullOrWhiteSpace(x.CurrentPassword));

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(256).WithMessage("Password must not exceed 256 characters.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character.");
    }
}