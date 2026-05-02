using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageOccurrenceRepository
{
    Task AddRangeAsync(List<PackageOccurrence> occurrences, CancellationToken cancellationToken);
    Task<bool> HasOverlappingOccurrenceAsync(Guid packageId, List<DateOnly> dates, CancellationToken cancellationToken);
    Task<bool> PackageExistsForUserAsync(Guid packageId, Guid userId, CancellationToken cancellationToken);
    Task<PackageOccurrence?> GetOccurrenceByPackageIdAndDate(Guid packageId, DateOnly date, CancellationToken cancellationToken);
}
