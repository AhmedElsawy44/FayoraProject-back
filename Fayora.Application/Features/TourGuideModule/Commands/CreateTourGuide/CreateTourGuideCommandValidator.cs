using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public class CreateTourGuideCommandValidator : AbstractValidator<CreateTourGuideCommand>
{
    public CreateTourGuideCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.")
            .MaximumLength(100).WithMessage("Device ID must not exceed 100 characters.");

        RuleFor(x => x.ProfessionalLicenseUrl)
            .NotEmpty().WithMessage("Professional license URL is required.")
            .Must(BeValidFileUrl).WithMessage("Professional license URL must be a valid absolute URL that starts with http or https.");
    }

    private static bool BeValidFileUrl(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}