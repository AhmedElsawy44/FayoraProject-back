namespace Fayora.Contracts.TouristModule
{
    public record GetAllLocationsResponse(
        List<LocationSummaryResponse> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public record LocationSummaryResponse(
        int Id,
        string Name,
        string MainImageUrl,
        decimal Rating,
        string Category);
}
