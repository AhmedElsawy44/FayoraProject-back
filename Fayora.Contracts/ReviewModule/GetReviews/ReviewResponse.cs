namespace Fayora.Contracts.ReviewModule.GetReviews;

public record ReviewResponse(
    Guid Id,
    Guid ReviewerId,
    string ReviewerName,
    string? ReviewerImageUrl,
    decimal Rating,
    string? Comment,
    DateTimeOffset CreatedAt,
    bool CanEdit
);
