namespace Fayora.Contracts.ReviewModule.CreateReview;

public record CreateReviewRequest(Guid BookingId, decimal Rating, string? Comment);
