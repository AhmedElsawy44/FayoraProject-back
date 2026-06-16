namespace Fayora.Contracts.BookingModule.CreatePackageBooking;

public sealed record CreatePackageBookingRequest
(
    DateOnly BookingDate,
    int Adults,
    int Children,
    string PaymentMethodType,
    string? WalletNumber,
    bool IsCashOnArrival,
    List<Guid>? SelectedOptionalActivityIds,
    Guid? SelectedMeetingPointId
);
