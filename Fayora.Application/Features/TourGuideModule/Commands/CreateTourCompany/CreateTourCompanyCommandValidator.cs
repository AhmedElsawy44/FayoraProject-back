using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;


public class CreateTourCompanyCommandValidator : AbstractValidator<CreateTourCompanyCommand>
{
    public CreateTourCompanyCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty().WithMessage("Main image URL is required.")
            .Must(BeValidFileUrl).WithMessage("Main image URL must be a valid absolute URL that starts with http or https.");

        RuleFor(x => x.LicenseDocumentUrl)
            .NotEmpty().WithMessage("License document URL is required.")
            .Must(BeValidFileUrl).WithMessage("License document URL must be a valid absolute URL that starts with http or https.");

        RuleFor(x => x.LicenseClass)
            .IsInEnum().WithMessage("Invalid license class.");
    }

    private static bool BeValidFileUrl(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
