using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageRepository
{
    void AddPackage(GuidePackage package);
    Task<GuidePackage?> GetPackageByIdAsync(Guid packageId, PackageQueryOptions options, CancellationToken cancellationToken);

    public record PackageQueryOptions(
        bool ReadOnly = true,
        bool IncludeOccurrences = false
    );
}
