namespace Fayora.Application.Features.AdminModule.Queries.GetTravelAgenciesStats;

public record TravelAgenciesStatsResponse(
    int ActiveAgencies,
    CombinedGmvDto CombinedGmv,
    AvgCommissionDto AvgCommission,
    int InOnboarding
);

public record CombinedGmvDto(decimal Amount, double GrowthPercentage);
public record AvgCommissionDto(double Rate, double PointChange);
