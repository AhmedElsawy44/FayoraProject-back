using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Commands.CreatePackageBooking;

public record BookingResult(Guid BookingId, string PaymentUrl);

public record CreatePackageBookingCommand(
    Guid PackageId,
    DateOnly BookingDate,
    int Adults,
    int Children,
    PaymentMethodType PaymentMethodType,
    string? WalletNumber,
    bool IsCashOnArrival,
    List<Guid>? SelectedOptionalActivityIds,
    Guid? SelectedMeetingPointId
    ) : ICommand<Result<BookingResult>>;
