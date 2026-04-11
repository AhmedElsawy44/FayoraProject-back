using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeactivateGuidePackage;

public class DeactivateGuidePackageCommandValidator : AbstractValidator<DeactivateGuidePackageCommand>
{
    public DeactivateGuidePackageCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("Tour Guide Package is required.");
    }
}
