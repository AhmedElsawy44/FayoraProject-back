namespace Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages
{
    public record GetMyPackagesResult(
        List<PackageSummaryResult> Items,
        int TotalCount,
        int Page,
        int PageSize
    );

    public record PackageSummaryResult(
        Guid Id,
        string Title,
        decimal AdultPrice,
        decimal ChildPrice,
        int DurationHours,
        int MaxCapacity,
        string MainImageUrl,
        string TourTypes,
        string PackageStatus
    );
}