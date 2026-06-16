namespace Fayora.Contracts.BookingModule.CreateGuideBooking
{
    public record CreateGuideBookingRequest(
        DateOnly BookingDate,
        int Adults,
        int Children,
        string PaymentMethodType,
        string? WalletNumber,
        bool IsCashOnArrival
    );
}
