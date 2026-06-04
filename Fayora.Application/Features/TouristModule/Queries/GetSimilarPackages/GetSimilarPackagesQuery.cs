using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetSimilarPackages;

public record GetSimilarPackagesQuery(
    Guid PackageId,
    int Count = 5
) : IQuery<Result<List<RecommendedPackageResult>>>;
