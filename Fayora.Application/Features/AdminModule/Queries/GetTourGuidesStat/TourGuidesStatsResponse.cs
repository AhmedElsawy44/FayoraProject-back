namespace Fayora.Application.Features.AdminModule.Queries.GetTourGuidesStat;

public record TourGuidesStatsResponse(
    int ActiveGuides,
    decimal AvgRating,
    int OnTourNow,
    GuideRevenueDto GuideRevenue
);
public record GuideRevenueDto(decimal Amount, double GrowthPercentage);
