using FluentValidation;

namespace Fayora.Application.Features.BookingModule.Commands.CreateGuideBooking
{
    public class CreateGuideBookingCommandValidator
        : AbstractValidator<CreateGuideBookingCommand>
    {
        public CreateGuideBookingCommandValidator()
        {
            RuleFor(x => x.GuideId)
                .NotEmpty();

            RuleFor(x => x.BookingDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Booking date must be today or in the future.");

            RuleFor(x => x.Adults)
                .GreaterThan(0)
                .WithMessage("At least one adult is required.");

            RuleFor(x => x.Children)
                .GreaterThanOrEqualTo(0);
        }
    }
}
