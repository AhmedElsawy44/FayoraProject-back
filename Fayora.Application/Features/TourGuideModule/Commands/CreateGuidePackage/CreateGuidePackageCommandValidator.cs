using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;

public class CreateGuidePackageCommandValidator : AbstractValidator<CreateGuidePackageCommand>
{
    public CreateGuidePackageCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.TourType)
            .IsInEnum().WithMessage("Invalid tour type.");

        RuleFor(x => x.DurationHours)
            .GreaterThan(0).WithMessage("Duration must be greater than zero.");

        RuleFor(x => x.Longitude)
            .GreaterThanOrEqualTo(0).WithMessage("Longitude must be non-negative.");

        RuleFor(x => x.Latitude)
            .GreaterThanOrEqualTo(0).WithMessage("Latitude must be non-negative.");

        RuleFor(x => x.TransportType)
            .IsInEnum().WithMessage("Invalid transport type.");

        RuleFor(x => x.AdultPrice)
            .GreaterThan(0).WithMessage("Adult price must be greater than zero.");

        RuleFor(x => x.ChildPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Child price cannot be negative.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Max capacity must be greater than zero.");

        RuleFor(x => x.ArrivalNote)
            .MaximumLength(1000).WithMessage("Arrival note cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ArrivalNote));

        RuleFor(x => x.MainImageUrl)
            .NotEmpty().WithMessage("Main image URL is required.")
            .Must(BeValidFileUrl).WithMessage("Main image URL must be a valid absolute URL that starts with http or https.");

        RuleFor(x => x.VideoURL)
            .Must(BeValidFileUrl).WithMessage("Video URL must be a valid absolute URL that starts with http or https.")
            .When(x => !string.IsNullOrWhiteSpace(x.VideoURL));

        RuleForEach(x => x.ImageURLs)
            .Must(BeValidFileUrl).WithMessage("Each image URL must be a valid absolute URL that starts with http or https.");

        RuleForEach(x => x.IncludedIds)
            .GreaterThan(0).WithMessage("Included item IDs must be greater than zero.");

        RuleForEach(x => x.ExcludedIds)
            .GreaterThan(0).WithMessage("Excluded item IDs must be greater than zero.");

        RuleFor(x => x.CancellationPolicy)
            .IsInEnum().WithMessage("Invalid cancellation policy.");

        RuleForEach(x => x.Activities)
            .ChildRules(activity =>
            {
                activity.RuleFor(a => a.Latitude)
                    .GreaterThanOrEqualTo(0).WithMessage("Activity latitude must be non-negative.");
                activity.RuleFor(a => a.Longitude)
                    .GreaterThanOrEqualTo(0).WithMessage("Activity longitude must be non-negative.");
                activity.RuleFor(a => a.Description)
                    .NotEmpty().WithMessage("Activity description is required.")
                    .MaximumLength(1000).WithMessage("Activity description cannot exceed 1000 characters.");
            });
    }

    private static bool BeValidFileUrl(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
