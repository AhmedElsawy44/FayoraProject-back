namespace Fayora.Contracts.TouristModule
{
    public record LocationDetailsResponse(
        int Id,
        string Name,
        string? Description,
        decimal Rating,
        string Category,
        string MainImageUrl,
        List<string> ImageUrls,
        decimal? Latitude,
        decimal? Longitude,
        List<LocationPackageSummaryResponse> Packages
    );

    public record LocationPackageSummaryResponse(
        Guid Id,
        string Title,
        decimal AdultPrice,
        int DurationHours,
        string MainImageUrl,
        string TourTypes,
        int Views
    );
}
