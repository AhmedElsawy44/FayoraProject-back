using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageRepository
{
    void AddPackage(GuidePackage package);
    Task<GuidePackage?> GetPackageByIdAsync(Guid packageId, PackageQueryOptions options, CancellationToken cancellationToken);

    //get all packages of a tour guide or tour company with pagination and filter by status
    Task<(List<GuidePackage> Items, int TotalCount)> GetMyPackagesAsync(
    Guid userId,
    ItemStatus? status,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default);

    //for tour guide's point of view
    Task<List<PackageActivity>> GetActivitiesByPackageIdAsync(
    Guid packageId,
    CancellationToken cancellationToken = default);

    void AddPackageActivities(IEnumerable<PackageActivity> activities);
    void RemovePackageActivities(IEnumerable<PackageActivity> activities);

    void AddPackageMeetingPoints(IEnumerable<PackageMeetingPoint> meetingPoints);
    void RemovePackageMeetingPoints(IEnumerable<PackageMeetingPoint> meetingPoints);
    Task<List<PackageMeetingPoint>> GetMeetingPointsByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<List<PackageMeetingPoint>> GetMeetingPointsByPackageIdsAsync(List<Guid> packageIds, CancellationToken cancellationToken = default);

    //for tourist's point of view
    Task<GuidePackage?> GetPackageWithOccurrencesAsync(
     Guid packageId,
     CancellationToken cancellationToken = default);


    public record PackageQueryOptions(
        bool ReadOnly = true,
        bool IncludeOccurrences = false,
        bool IncludeMeetingPoints = false
    );

    Task<List<GuidePackage>> GetListByIdsAsync(List<Guid> packageIds, CancellationToken cancellationToken);

    Task<bool> HasPackageCreatedTodayAsync(Guid userId, CancellationToken cancellationToken);

    Task<(List<GuidePackage> Items, int TotalCount)> GetActivePackagesAsync(
    string? search,
    int? locationId,
    ProviderType? providerType,
    ItemStatus? tourType,
    int? minDuration,
    int? maxDuration,
    decimal? minPrice,
    decimal? maxPrice,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default);
}
