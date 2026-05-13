using FluentValidation;

namespace Fayora.Application.Features.BookingModule.Commands.CreateAccommodationBooking
{
    public class CreateAccommodationBookingCommandValidator : AbstractValidator<CreateAccommodationBookingCommand>
    {
        public CreateAccommodationBookingCommandValidator()
        {
            RuleFor(x => x.UnitId)
                .NotEmpty();

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Start date must be today or in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");

            RuleFor(x => x.Adults)
                .GreaterThan(0)
                .WithMessage("At least one adult is required.");

            RuleFor(x => x.Children)
                .GreaterThanOrEqualTo(0);
        }
    }
}
