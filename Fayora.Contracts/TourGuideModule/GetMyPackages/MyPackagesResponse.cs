namespace Fayora.Contracts.TourGuideModule.GetMyPackages
{
    public record MyPackagesResponse(
        List<PackageSummaryResponse> Items,
        int TotalCount,
        int Page,
        int PageSize
    );

    public record PackageSummaryResponse(
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
