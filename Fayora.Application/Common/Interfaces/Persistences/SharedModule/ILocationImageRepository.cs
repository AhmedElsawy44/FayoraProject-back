using Fayora.Domain.Entities.SharedModule;

namespace Fayora.Application.Common.Interfaces.Persistences.SharedModule
{
    public interface ILocationImageRepository
    {
        void AddLocationImages(List<LocationImage> images);
        Task<List<LocationImage>> GetImagesByLocationIdAsync(
            int locationId,
            CancellationToken cancellationToken = default);
    }
}
