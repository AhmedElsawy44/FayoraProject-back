namespace Fayora.Contracts.BookingModule.GetBookingDetails
{
    public record BookingDetailsResponse(
        Guid BookingId,
        string Title,
        string ImageUrl,
        decimal TotalPrice,
        int GuestsCount,
        DateTime StartDate,
        DateTime EndDate,
        string Status,
        string ServiceType,
        string? QrToken
    );
}
