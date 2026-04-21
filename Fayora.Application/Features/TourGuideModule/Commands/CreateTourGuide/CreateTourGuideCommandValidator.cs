using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public class CreateTourGuideCommandValidator : AbstractValidator<CreateTourGuideCommand>
{
    public CreateTourGuideCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        // 2. Profile Picture
        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty().WithMessage("Profile picture URL is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Profile picture must be a valid URL.");


        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(50).WithMessage("Description must be at least 50 characters long to provide a good profile.");


        RuleFor(x => x.PricingUnit)
            .IsInEnum().WithMessage("Invalid pricing unit.");

        RuleFor(x => x.BaseRate)
            .GreaterThan(0).WithMessage("Base rate must be greater than zero.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(60).WithMessage("Years of experience seems invalid.");

        RuleFor(x => x.NationalityCode)
            .Length(2, 3).WithMessage("Nationality code must be 2 or 3 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalityCode));


        RuleFor(x => x.CityIds)
            .NotEmpty().WithMessage("At least one city must be selected.")
            .Must(cities => cities != null && cities.Count > 0).WithMessage("City list cannot be empty.");

        RuleFor(x => x.PreferredLanguage)
            .IsInEnum().WithMessage("Invalid preferred language.")
            .When(x => x.PreferredLanguage.HasValue);

        RuleFor(x => x.TourGuideLanguages)
            .NotEmpty().WithMessage("At least one tour guide language must be provided.");

        RuleForEach(x => x.TourGuideLanguages).ChildRules(language =>
        {
            language.RuleFor(l => l.Language)
                .IsInEnum().WithMessage("Invalid language.");
            language.RuleFor(l => l.ProficiencyLevel)
                .InclusiveBetween(0m, 1m).WithMessage("Proficiency level must be between 0 and 1.");
        });
    }
}