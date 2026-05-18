namespace Fayora.Application.Features.AdminModule.Queries.GetInventoryStats;

public record InventoryStatsResponse(
    int LiveListings,
    int PendingReview,
    double AvgOccupancy,
    int FlaggedCount
);

