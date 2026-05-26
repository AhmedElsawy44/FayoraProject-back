namespace Fayora.Contracts.TouristModule
{
    public record ActivePackagesResponse(
        List<ActivePackageSummaryResponse> Items,
        int TotalCount,
        int Page,
        int PageSize
    );

    public record ActivePackageSummaryResponse(
        Guid Id,
        string Title,
        decimal AdultPrice,
        int DurationHours,
        string MainImageUrl,
        string TourTypes,
        decimal GuideRating,
        int Views
    );
}
