using FluentValidation;

namespace Fayora.Application.Features.AdminModule.Commands.CreateMasterAmenity;

public class CreateMasterAmenityCommandValidator : AbstractValidator<CreateMasterAmenityCommand>
{
    public CreateMasterAmenityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Amenity name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("A valid amenity category must be selected.");

        RuleFor(x => x.IconUrl)
            .MaximumLength(2048).WithMessage("Icon URL is too long; it cannot exceed 2048 characters.");
    }
}