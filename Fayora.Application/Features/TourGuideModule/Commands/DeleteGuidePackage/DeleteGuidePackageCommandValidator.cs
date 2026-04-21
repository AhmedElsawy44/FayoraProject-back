using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;

public class DeleteGuidePackageCommandValidator : AbstractValidator<DeleteGuidePackageCommand>
{
    public DeleteGuidePackageCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("Tour Guide Package is required.");
    }
}
