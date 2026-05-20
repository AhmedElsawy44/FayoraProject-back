namespace Fayora.Application.Features.BookingModule.Commands.ScanBookingQr
{
    public record ScanBookingQrResult(
        bool IsValid,
        string CustomerName,
        string ServiceName,
        string ServiceType,
        DateTime BookingDate,
        int NumberOfGuests,
        string Status,
        bool IsCashOnArrival, // ده في حالة انو هيدفع كاش عند الوصول => هيعرضلو المبلغ المتبقي اللي لازم يدفعه وهو يعمل سكان
       decimal? RemainingAmount
    );
}