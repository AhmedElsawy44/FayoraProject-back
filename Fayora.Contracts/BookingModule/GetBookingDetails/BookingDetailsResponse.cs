namespace Fayora.Contracts.BookingModule.GetBookingDetails
{
    public record BookingDetailsResponse(
        Guid BookingId,
        string Title,
        string ImageUrl,
        decimal BasePrice,
        decimal DiscountAmount,
        decimal TotalPrice,
        int GuestsCount,
        DateTime StartDate,
        DateTime EndDate,
        string Status,
        string ServiceType,
        string? QrToken,
        List<SelectedOptionalActivityResponse>? SelectedOptionalActivities,
        BookingMeetingPointResponse? SelectedMeetingPoint
    );

    public record SelectedOptionalActivityResponse(
        Guid Id,
        string Description,
        decimal AdditionalPrice,
        string ImageUrl
    );

    public record BookingMeetingPointResponse(
        Guid Id,
        string? MeetingPointName,
        decimal Latitude,
        decimal Longitude,
        TimeOnly Time,
        decimal Price,
        string? Description
    );
}
