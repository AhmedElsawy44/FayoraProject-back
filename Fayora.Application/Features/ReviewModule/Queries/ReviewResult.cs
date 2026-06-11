namespace Fayora.Application.Features.ReviewModule.Queries;

public record ReviewResult(
    Guid Id,
    Guid ReviewerId,
    string ReviewerName,
    string? ReviewerImageUrl,
    decimal Rating,
    string? Comment,
    DateTimeOffset CreatedAt,
    bool CanEdit
);
