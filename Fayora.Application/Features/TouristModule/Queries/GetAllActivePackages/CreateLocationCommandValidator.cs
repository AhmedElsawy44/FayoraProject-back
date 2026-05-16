using Fayora.Application.Features.AdminModule.Commands.CreateLocation;
using FluentValidation;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{
    public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
    {

        public class GetActivePackagesQueryValidator : AbstractValidator<GetActivePackagesQuery>
        {
            public GetActivePackagesQueryValidator()
            {
                RuleFor(x => x.Page)
                    .GreaterThan(0).WithMessage("Page must be greater than 0.");

                RuleFor(x => x.PageSize)
                    .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

                RuleFor(x => x.MinPrice)
                    .GreaterThan(0).When(x => x.MinPrice.HasValue)
                    .WithMessage("Min price must be greater than 0.");

                RuleFor(x => x.MaxPrice)
                    .GreaterThan(x => x.MinPrice ?? 0).When(x => x.MaxPrice.HasValue)
                    .WithMessage("Max price must be greater than min price.");

                RuleFor(x => x.MinDuration)
                    .GreaterThan(0).When(x => x.MinDuration.HasValue)
                    .WithMessage("Min duration must be greater than 0.");
            }
        }
    }
}
