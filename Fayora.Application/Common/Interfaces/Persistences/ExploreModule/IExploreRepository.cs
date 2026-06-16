using Fayora.Contracts.ExploreModule;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Common.Interfaces.Persistences.ExploreModule;

public interface IExploreRepository
{
    Task<List<ExploreItemDto>> GetExploreItemsAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<List<ExploreItemResponse>> GetExploreItemsAsync(
        string? search,
        string? type,
        List<Guid> wishlistIds,
        CancellationToken cancellationToken);
}
