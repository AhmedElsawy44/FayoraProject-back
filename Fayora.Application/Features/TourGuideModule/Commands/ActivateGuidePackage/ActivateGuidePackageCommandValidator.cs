using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;

public class ActivateGuidePackageCommandValidator : AbstractValidator<ActivateGuidePackageCommand>
{
    public ActivateGuidePackageCommandValidator()
    {
        RuleFor(x => x.PackageId)
        .NotEmpty().WithMessage("Tour Guide Package is required.");
    }
}
