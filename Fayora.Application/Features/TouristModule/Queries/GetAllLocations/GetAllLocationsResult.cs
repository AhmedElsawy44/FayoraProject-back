namespace Fayora.Application.Features.TouristModule.Queries.GetAllLocations
{
    public record GetAllLocationsResult(
        List<LocationSummaryResult> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public record LocationSummaryResult(
        int Id,
        string Name,
        string MainImageUrl,
        decimal Rating,
        string Category);
}