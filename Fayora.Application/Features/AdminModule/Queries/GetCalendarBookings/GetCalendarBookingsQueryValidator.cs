using FluentValidation;

namespace Fayora.Application.Features.AdminModule.Queries.GetCalendarBookings;

public class GetCalendarBookingsQueryValidator : AbstractValidator<GetCalendarBookingsQuery>
{
    public GetCalendarBookingsQueryValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("The year is required.")
            .GreaterThanOrEqualTo(2025).WithMessage("The year is cannot be more than current year.");

        RuleFor(x => x.Month)
            .NotEmpty().WithMessage("The month is required.")
            .InclusiveBetween(1, 12).WithMessage("The month must be between 1 and 12.");
    }
}