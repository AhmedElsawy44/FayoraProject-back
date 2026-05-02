using FluentValidation;

namespace Fayora.Application.Features.BookingModule.Commands.CreatePackageBooking;

public class CreatePackageBookingCommandValidator : AbstractValidator<CreatePackageBookingCommand>
{
    public CreatePackageBookingCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("PackageId is required.");

        RuleFor(x => x.BookingDate)
            .Must(d => d != default).WithMessage("Booking date is required.");

        RuleFor(x => x.Adults)
            .GreaterThan(0).WithMessage("Adults must be greater than zero.");

        RuleFor(x => x.Children)
            .GreaterThanOrEqualTo(0).WithMessage("Children cannot be negative.");

        RuleFor(x => x)
            .Must(x => x.Adults + x.Children > 0)
            .WithMessage("At least one guest is required.");

        RuleFor(x => x.PaymentMethodType)
            .IsInEnum().WithMessage("Invalid payment method.");
    }
}
