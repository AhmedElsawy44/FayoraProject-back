namespace Fayora.Application.Features.TouristModule.Queries.GetLocationDetails
{

    public record GetLocationDetailsResult(
        int Id,
        string Name,
        string? Description,
        decimal Rating,
        string Category,
        string MainImageUrl,
        List<string> ImageUrls,
        decimal? Latitude,
        decimal? Longitude,
        List<LocationPackageSummaryResult> Packages
    );

    public record LocationPackageSummaryResult(
        Guid Id,
        string Title,
        decimal AdultPrice,
        int DurationHours,
        string MainImageUrl,
        string TourTypes,
        int Views
    );
}