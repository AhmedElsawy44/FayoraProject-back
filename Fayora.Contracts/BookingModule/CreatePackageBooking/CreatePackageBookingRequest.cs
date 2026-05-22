namespace Fayora.Contracts.BookingModule.CreatePackageBooking;

public record CreatePackageBookingRequest
(
    DateOnly BookingDate,
    int Adults,
    int Children,
    string PaymentMethodType,
    string? WalletNumber,
    bool IsCashOnArrival
);


