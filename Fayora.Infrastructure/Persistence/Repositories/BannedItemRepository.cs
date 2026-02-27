using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class BannedItemRepository(ApplicationDbContext context) : BaseRepository<BannedItem, int>(context), IBannedItemRepository
{
    public Task<bool> IsBannedAsync(BanType banType, string banValue, CancellationToken cancellationToken) => IsExistAsync(d => d.BanType == banType && d.BanValue == banValue, cancellationToken);
}
