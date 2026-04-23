//namespace Fayora.Application.Features.TourGuideModule.Queries.GetAllPackages
//{
//    public record GetAllPackagesResult(
//        List<PackageItemResult> Items,
//        int TotalCount,
//        int Page,
//        int PageSize,
//        int TotalPages);

//    public record PackageItemResult(
//        Guid PackageId,
//        Guid CompanyId,
//        string CompanyName,
//        string Title,
//        string Description,
//        int TourTypes,
//        int DurationHours,
//        int MaxCapacity,
//        int AvailableSpots,
//        decimal AdultPrice,
//        decimal ChildPrice,
//        string? MainImageUrl,
//        DateOnly StartDate,
//        DateOnly EndDate);
//}