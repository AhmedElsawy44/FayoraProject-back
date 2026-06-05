using Fayora.Contracts.ExploreModule;

namespace Fayora.Application.Common.Interfaces.Persistences.ExploreModule;

public interface IExploreRepository
{
    Task<List<ExploreItemDto>> GetExploreItemsAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
