using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
            .When(x => x.LastName != null);

        RuleFor(x => x.BirthDate)
            .Must(BeAValidAge).WithMessage("You must be at least 18 years old.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender selection.")
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.NationalityCode)
            .Length(2, 3).WithMessage("Nationality code must be 2 or 3 characters.")
            .When(x => x.NationalityCode != null);

        RuleFor(x => x.ProfileImageUrl)
            .Must(BeAValidUrl).WithMessage("Profile image must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.PreferredLanguage)
            .Length(2).WithMessage("Preferred language must be a 2-character ISO code (e.g., 'en', 'ar').")
            .When(x => x.PreferredLanguage != null);

        RuleFor(x => x.TimeZone)
            .MaximumLength(100).WithMessage("Time zone string is too long.")
            .When(x => x.TimeZone != null);
    }

    private bool BeAValidAge(DateOnly? date)
    {
        if (!date.HasValue) return true;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minAllowedBirthDate = today.AddYears(-18);

        return date.Value <= minAllowedBirthDate;
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;

        return false;
    }
}