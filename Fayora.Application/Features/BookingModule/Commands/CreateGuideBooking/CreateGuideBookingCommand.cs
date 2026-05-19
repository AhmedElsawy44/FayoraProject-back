using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Commands.CreateGuideBooking
{
    public record CreateGuideBookingCommand(
        Guid GuideId,
        DateOnly BookingDate,
        TimeSpan StartTime,
        int Adults,
        int Children,
        PaymentMethodType PaymentMethodType,
        string? WalletNumber
    ) : ICommand<Result<string>>;
}
