namespace Fayora.Application.Common.Interfaces.Persistences.AdminModule;

public interface IInventoryModerationService
{
    Task<List<InventoryQueueItemDto>> GetInventoryQueueAsync(TypeFilter? typeFilter, int page, int pageSize, CancellationToken ct);
}

public record InventoryQueueItemDto
(
    Guid Id,
    string Title,
    string Type,
    string ProviderName,
    decimal Price,
    string PriceUnit,
    string ImageUrl
);

public enum TypeFilter
{
    All,
    Trips,
    Accommodations,
}
