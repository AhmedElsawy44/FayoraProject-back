namespace Fayora.Contracts.BookingModule.CreateUnitBooking
{
    public record CreateAccommodationBookingRequest(
        DateOnly StartDate,
        DateOnly EndDate,
        int Adults,
        int Children,
        string PaymentMethodType
    );
}
