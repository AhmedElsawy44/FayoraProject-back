using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;

/// <summary>
/// Query to retrieve recommended packages for the homepage.
/// Works for both authenticated users (personalized) and anonymous users (trending).
/// </summary>
public record GetRecommendedPackagesQuery(
    int Count = 10
) : IQuery<Result<List<RecommendedPackageResult>>>;
