using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages
{
    public record GetActivePackagesQuery(
        string? Search,
        int? LocationId,
        ProviderType? ProviderType,
        TourType? TourType,
        int? MinDuration,
        int? MaxDuration,
        decimal? MinPrice,
        decimal? MaxPrice,
        int Page,
        int PageSize) : IQuery<Result<GetActivePackagesResult>>;
}
