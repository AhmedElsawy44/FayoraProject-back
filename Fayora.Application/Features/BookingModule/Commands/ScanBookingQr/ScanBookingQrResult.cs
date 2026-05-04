namespace Fayora.Application.Features.BookingModule.Commands.ScanBookingQr
{
    public record ScanBookingQrResult(
        bool IsValid,
        string CustomerName,
        string ServiceName,
        string ServiceType,
        DateTime BookingDate,
        int NumberOfGuests,
        string Status
    );
}