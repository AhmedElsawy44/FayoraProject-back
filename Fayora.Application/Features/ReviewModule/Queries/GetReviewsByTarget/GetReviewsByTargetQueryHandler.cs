using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.ReviewModule;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.ReviewModule.Queries.GetReviewsByTarget;

public class GetReviewsByTargetQueryHandler(
    IReviewRepository reviewRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetReviewsByTargetQuery, Result<List<ReviewResult>>>
{
    public async Task<Result<List<ReviewResult>>> Handle(
        GetReviewsByTargetQuery request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ReviewTargetType>(request.TargetType, true, out var targetTypeEnum))
        {
            return ReviewErrors.InvalidTargetType;
        }

        Guid currentUserId = Guid.Empty;
        var context = clientContextProvider.GetContext();
        if (context is not null)
        {
            currentUserId = context.UserId;
        }

        var reviews = await reviewRepository.GetByTargetAsync(
            request.TargetId,
            targetTypeEnum,
            request.Page,
            request.PageSize,
            cancellationToken);

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
                currentUserId != Guid.Empty && review.ReviewerId == currentUserId && review.CanModify()
            );
        }).ToList();

        return results;
    }
}
