namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{

    public record GetActivePackagesResult(
        List<ActivePackageSummaryResult> Items,
        int TotalCount,
        int Page,
        int PageSize
    );

    public record ActivePackageSummaryResult(
        Guid Id,
        string Title,
        decimal AdultPrice,
        decimal DiscountedAdultPrice,
        int DurationHours,
        string MainImageUrl,
        string TourTypes,
        decimal GuideRating,
        int Views,
        string? ProviderName = null,
        string? ProviderImageUrl = null,
        int? ProviderType = null
    );
}