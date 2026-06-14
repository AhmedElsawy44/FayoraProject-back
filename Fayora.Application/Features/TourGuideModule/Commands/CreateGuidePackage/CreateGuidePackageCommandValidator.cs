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
        RuleFor(x => x.MeetingPoints)
            .NotEmpty().WithMessage("At least one meeting point is required.");

        RuleForEach(x => x.MeetingPoints)
            .ChildRules(mp =>
            {
                mp.RuleFor(a => a.MeetingPointName)
                    .NotEmpty().WithMessage("Meeting point name is required.");
                mp.RuleFor(a => a.Latitude)
                    .GreaterThanOrEqualTo(-90).LessThanOrEqualTo(90).WithMessage("Latitude must be between -90 and 90.");
                mp.RuleFor(a => a.Longitude)
                    .GreaterThanOrEqualTo(-180).LessThanOrEqualTo(180).WithMessage("Longitude must be between -180 and 180.");
                mp.RuleFor(a => a.Time)
                    .Must(t => t != default).WithMessage("Meeting point time is required.");
                mp.RuleFor(a => a.Price)
                    .GreaterThanOrEqualTo(0).WithMessage("Meeting point price cannot be negative.");
            });

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
                activity.RuleFor(a => a.Description)
                    .NotEmpty().WithMessage("Activity description is required.")
                    .MaximumLength(1000).WithMessage("Activity description cannot exceed 1000 characters.");

                activity.RuleFor(a => a)
                    .Must(a => a.LocationId.HasValue || (a.Latitude.HasValue && a.Longitude.HasValue))
                    .WithMessage("Activity must have either a LocationId or both Latitude and Longitude.");

                activity.RuleFor(a => a.Latitude)
                    .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
                    .When(a => a.Latitude.HasValue);

                activity.RuleFor(a => a.Longitude)
                    .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
                    .When(a => a.Longitude.HasValue);

                activity.RuleFor(a => a.Latitude)
                    .NotNull().WithMessage("Latitude is required when Longitude is provided.")
                    .When(a => a.Longitude.HasValue);

                activity.RuleFor(a => a.Longitude)
                    .NotNull().WithMessage("Longitude is required when Latitude is provided.")
                    .When(a => a.Latitude.HasValue);
            });

        RuleForEach(x => x.OptionalActivities)
            .ChildRules(optAct =>
            {
                optAct.RuleFor(a => a.Description)
                    .NotEmpty().WithMessage("Optional activity description is required.")
                    .MaximumLength(1000).WithMessage("Optional activity description cannot exceed 1000 characters.");
                optAct.RuleFor(a => a.AdditionalPrice)
                    .GreaterThanOrEqualTo(0).WithMessage("Optional activity additional price cannot be negative.");
                optAct.RuleFor(a => a.ImageUrl)
                    .NotEmpty().WithMessage("Optional activity image URL is required.")
                    .Must(BeValidFileUrl).WithMessage("Optional activity image URL must be a valid absolute URL that starts with http or https.");
            })
            .When(x => x.OptionalActivities != null);
    }

    private static bool BeValidFileUrl(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
