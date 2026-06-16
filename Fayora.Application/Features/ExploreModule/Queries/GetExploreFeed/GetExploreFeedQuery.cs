using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.ExploreModule;
using Fayora.Domain.Common.Results;
using System.Collections.Generic;

namespace Fayora.Application.Features.ExploreModule.Queries.GetExploreFeed;

public record GetExploreFeedQuery(
    int PageNumber,
    int PageSize,
    string? Search,
    string? Type) : IQuery<Result<List<ExploreItemResponse>>>;
