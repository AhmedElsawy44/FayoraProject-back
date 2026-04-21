using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.BirthDate)
            .Must(BeAValidAge).WithMessage("You must be at least 18 years old.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender.")
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.PreferredLanguage)
            .IsInEnum().WithMessage("Invalid preferred language.")
            .When(x => x.PreferredLanguage.HasValue);

        RuleForEach(x => x.UserLanguages)
            .ChildRules(lang =>
            {
                lang.RuleFor(x => x.Language)
                    .IsInEnum().WithMessage("Invalid user language.");

                lang.RuleFor(x => x.ProficiencyLevel)
                    .InclusiveBetween(0m, 1m).WithMessage("Invalid language proficiency level. It should be between 0 and 1.");
            });

        RuleFor(x => x.ProfileImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("Profile image URL must be a valid absolute URL.")
            .MaximumLength(2048).WithMessage("Profile image URL is too long.")
            .When(x => !string.IsNullOrWhiteSpace(x.ProfileImageUrl));

        RuleFor(x => x.NationalityCode)
            .Length(2, 3).WithMessage("Nationality code must be 2 or 3 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalityCode));

        RuleFor(x => x.UserLanguages)
            .NotNull().WithMessage("User languages list is required.");

        RuleFor(x => x.TimeZone)
            .MaximumLength(50).WithMessage("Time zone string is too long.")
            .When(x => x.TimeZone != null);
    }

    private bool BeAValidAge(DateOnly? date)
    {
        if (!date.HasValue) return true;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minAllowedBirthDate = today.AddYears(-18);

        return date.Value <= minAllowedBirthDate;
    }

}