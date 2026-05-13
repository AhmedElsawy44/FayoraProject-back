using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageRepository
{
    void AddPackage(GuidePackage package);
    Task<GuidePackage?> GetPackageByIdAsync(Guid packageId, PackageQueryOptions options, CancellationToken cancellationToken);

    //for tour guide's point of view
    Task<List<PackageActivity>> GetActivitiesByPackageIdAsync(
    Guid packageId,
    CancellationToken cancellationToken = default);


    //for tourist's point of view
    Task<GuidePackage?> GetPackageWithOccurrencesAsync(
     Guid packageId,
     CancellationToken cancellationToken = default);


    public record PackageQueryOptions(
        bool ReadOnly = true,
        bool IncludeOccurrences = false
    );

    Task<List<GuidePackage>> GetListByIdsAsync(List<Guid> packageIds, CancellationToken cancellationToken);
}
