using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule
{
    public interface IPackageAccommodationRepository
    {
        void Add(PackageAccommodation accommodation);
        Task<PackageAccommodation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }

}
