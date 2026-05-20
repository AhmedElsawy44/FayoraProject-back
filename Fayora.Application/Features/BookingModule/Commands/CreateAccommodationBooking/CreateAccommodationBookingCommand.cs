using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Commands.CreateAccommodationBooking
{
    public record CreateAccommodationBookingCommand(
        Guid UnitId,
        DateOnly StartDate,
        DateOnly EndDate,
        int Adults,
        int Children,
        PaymentMethodType PaymentMethodType,
        string? WalletNumber,
        bool IsCashOnArrival
    ) : ICommand<Result<string>>;
}
