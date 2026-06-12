using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Queries.GetMyReviews;

public class GetMyReviewsQueryHandler(
    IReviewRepository reviewRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetMyReviewsQuery, Result<List<ReviewResult>>>
{
    public async Task<Result<List<ReviewResult>>> Handle(
        GetMyReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var reviews = await reviewRepository.GetMyReviewsAsync(userId, cancellationToken);

        var results = reviews.Select(review =>
        {
            var reviewerName = review.Reviewer != null ? review.Reviewer.FullName : "Anonymous";
            var reviewerImageUrl = review.Reviewer?.ProfileImageUrl?.ToString();

            return new ReviewResult(
                review.Id,
                review.ReviewerId,
                reviewerName,
                reviewerImageUrl,
                review.Rating,
                review.Comment,
                review.CreatedAt,
                review.CanModify() // Since the tourist is querying their own reviews, ownership check is implicit. They just need to be within the 48-hour window.
            );
        }).ToList();

        return results;
    }
}
