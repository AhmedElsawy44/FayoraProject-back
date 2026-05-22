using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Queries.GetBookingDetails
{
    public record GetBookingDetailsResult(
        Guid BookingId,
        string Title,
        string ImageUrl,
        decimal TotalPrice,
        int GuestsCount,
        DateTime StartDate,
        DateTime EndDate,
        BookingStatus Status,
        ServiceType ServiceType,
        string? QrToken // nullable bec qr will generate only BookingStatus be paid 
    );
}