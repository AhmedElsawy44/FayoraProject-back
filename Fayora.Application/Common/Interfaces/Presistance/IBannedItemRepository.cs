using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IBannedItemRepository
{
    public Task<bool> IsBannedAsync(BanType banType, string banValue, CancellationToken cancellationToken);
}
