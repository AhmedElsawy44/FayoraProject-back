using Fayora.Application.Common.Abstractions.Messaging;

namespace Fayora.Application.Features.AdminModule.Queries.GetInventoryStats;

public record GetInventoryStatsQuery : IQuery<InventoryStatsResponse>;
