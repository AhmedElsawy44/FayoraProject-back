using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Queries.GetBookingDetails
{
    public record GetBookingDetailsResult(
        Guid BookingId,
        string Title,
        string ImageUrl,
        decimal BasePrice,       // السعر الأصلي قبل أي خصم  
        decimal DiscountAmount,  // قيمة الخصم اللي اتطبق  
        decimal TotalPrice,      // اللي المستخدم دفعه فعلاً  
        int GuestsCount,         // إجمالي عدد الضيوف (Adults + Children)
        int AdultsCount,
        int ChildrenCount,
        DateTime StartDate,
        DateTime EndDate,
        BookingStatus Status,
        ServiceType ServiceType,
        string? QrToken, // nullable bec qr will generate only BookingStatus be paid 
        List<SelectedOptionalActivityResult>? SelectedOptionalActivities = null
    );

    public record SelectedOptionalActivityResult(
        Guid Id,
        string Description,
        decimal AdditionalPrice,
        string ImageUrl
    );
}