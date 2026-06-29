using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Queries.GetReviewsByTarget;

public record GetReviewsByTargetQuery(Guid TargetId, string TargetType, int Page = 1, int PageSize = 10)
    : IQuery<Result<List<ReviewResult>>>;
