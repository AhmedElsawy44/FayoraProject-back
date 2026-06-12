namespace Fayora.Contracts.ReviewModule.GetReviews;

public record GetReviewsResponse(
    List<ReviewResponse> Reviews,
    int Page,
    int PageSize
);
