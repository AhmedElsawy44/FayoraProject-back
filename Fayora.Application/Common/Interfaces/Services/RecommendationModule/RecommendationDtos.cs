using Fayora.Application.Features.AccommodationModule.Queries.GetRecommendedUnits;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;

namespace Fayora.Application.Common.Interfaces.Services.RecommendationModule;

public record AllRecommendationsResult(
    List<RecommendedUnitResult> Housing,
    List<RecommendedGuideResult> Guides,
    List<RecommendedPackageResult> Packages
);

public record PythonRefreshResult(
    string Status,
    int Housing,
    int Guides,
    int Packages,
    int Tourists,
    int Interactions,
    double ElapsedSec
);
