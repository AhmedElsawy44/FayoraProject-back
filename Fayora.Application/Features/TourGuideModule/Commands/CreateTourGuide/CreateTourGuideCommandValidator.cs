using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public class CreateTourGuideCommandValidator : AbstractValidator<CreateTourGuideCommand>
{
    public CreateTourGuideCommandValidator()
    {
        // 1. Device ID
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        // 2. Profile Picture
        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty().WithMessage("Profile picture URL is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Profile picture must be a valid URL.");

        // 3. Description (يفضل أن يكون طويلاً نسبياً للمرشد السياحي)
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(50).WithMessage("Description must be at least 50 characters long to provide a good profile.");

        // 4. Pricing & Rates
        RuleFor(x => x.PricingUnit)
            .NotEmpty().WithMessage("Pricing unit is required.");

        RuleFor(x => x.BaseRate)
            .GreaterThan(0).WithMessage("Base rate must be greater than zero.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3).WithMessage("Currency code must be exactly 3 characters (e.g., EGP, USD).");

        // 5. Experience
        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(60).WithMessage("Years of experience seems invalid.");

        // 6. License Verification
        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.");

        RuleFor(x => x.LicenseExpiryDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("License expiry date must be in the future.");

        // 7. Location (Cities)
        RuleFor(x => x.CityIds)
            .NotEmpty().WithMessage("At least one city must be selected.")
            .Must(cities => cities != null && cities.Count > 0).WithMessage("City list cannot be empty.");

        // 8. Languages
        RuleFor(x => x.PreferredLanguage)
            .NotEmpty().WithMessage("Preferred language is required.");

        RuleFor(x => x.TourGuideLanguages)
            .NotEmpty().WithMessage("At least one tour guide language must be provided.");

        RuleForEach(x => x.TourGuideLanguages).ChildRules(language =>
        {
            language.RuleFor(l => l.Language)
                .NotEmpty().WithMessage("Language name is required.");
            language.RuleFor(l => l.ProficiencyLevel)
                .Must(level => level >= 0 && level <= 1).WithMessage("Proficiency level must be between 0 and 1.");
        });
    }
}