using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Queries.GetMyReviews;

public record GetMyReviewsQuery() : IQuery<Result<List<ReviewResult>>>;
