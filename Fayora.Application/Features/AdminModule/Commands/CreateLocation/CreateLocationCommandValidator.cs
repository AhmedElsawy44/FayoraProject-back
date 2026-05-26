using FluentValidation;

namespace Fayora.Application.Features.AdminModule.Commands.CreateLocation
{

    public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
    {
        public CreateLocationCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Invalid latitude.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Invalid longitude.");

            RuleFor(x => x.MainImageUrl)
                .NotEmpty().WithMessage("Main image is required.");
        }
    }
}
