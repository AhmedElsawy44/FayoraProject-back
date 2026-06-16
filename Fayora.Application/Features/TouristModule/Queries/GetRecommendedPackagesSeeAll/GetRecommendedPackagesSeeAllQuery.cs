using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackagesSeeAll;

public record GetRecommendedPackagesSeeAllQuery(
    int Page = 1,
    int Size = 10
) : IQuery<Result<List<RecommendedPackageResult>>>;
