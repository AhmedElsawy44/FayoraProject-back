using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.ReviewModule;
using Fayora.Domain.Common.Events.ReviewModule;
using Fayora.Domain.Errors;
using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Domain.Entities.ReviewModule;

public class Review : AuditableEntity<Guid>
{
    public Guid BookingId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public User? Reviewer { get; private set; }
    public Guid TargetId { get; private set; }
    public ReviewTargetType TargetType { get; private set; }
    public decimal Rating { get; private set; }
    public string? Comment { get; private set; }
    public bool IsDeleted { get; private set; }

    private Review(
        Guid id,
        Guid bookingId,
        Guid reviewerId,
        Guid targetId,
        ReviewTargetType targetType,
        decimal rating,
        string? comment)
    {
        Id = id;
        BookingId = bookingId;
        ReviewerId = reviewerId;
        TargetId = targetId;
        TargetType = targetType;
        Rating = rating;
        Comment = comment;
        IsDeleted = false;
    }

    private Review() { }

    public static Result<Review> Create(
        Guid bookingId,
        Guid reviewerId,
        Guid targetId,
        ReviewTargetType targetType,
        decimal rating,
        string? comment)
    {
        var ratingValidation = ValidateRating(rating);
        if (ratingValidation.IsError)
        {
            return ratingValidation.Errors;
        }

        var commentValidation = ValidateComment(comment);
        if (commentValidation.IsError)
        {
            return commentValidation.Errors;
        }

        var review = new Review(
            Guid.CreateVersion7(),
            bookingId,
            reviewerId,
            targetId,
            targetType,
            rating,
            comment);

        review.RaiseDomainEvent(new ReviewCreatedEvent(
            review.Id,
            review.TargetId,
            review.TargetType,
            review.Rating));

        return review;
    }

    public Result<Success> Update(decimal rating, string? comment)
    {
        if (!CanModify())
        {
            return ReviewErrors.EditWindowExpired;
        }

        var ratingValidation = ValidateRating(rating);
        if (ratingValidation.IsError)
        {
            return ratingValidation.Errors;
        }

        var commentValidation = ValidateComment(comment);
        if (commentValidation.IsError)
        {
            return commentValidation.Errors;
        }

        var oldRating = Rating;
        Rating = rating;
        Comment = comment;
        Updated();

        RaiseDomainEvent(new ReviewUpdatedEvent(
            Id,
            TargetId,
            TargetType,
            oldRating,
            rating));

        return Result.Success;
    }

    public Result<Success> Delete()
    {
        if (!CanModify())
        {
            return ReviewErrors.EditWindowExpired;
        }

        IsDeleted = true;
        Updated();

        RaiseDomainEvent(new ReviewDeletedEvent(
            Id,
            TargetId,
            TargetType,
            Rating));

        return Result.Success;
    }

    public Result<Success> AdminDelete()
    {
        IsDeleted = true;
        Updated();

        RaiseDomainEvent(new ReviewDeletedEvent(
            Id,
            TargetId,
            TargetType,
            Rating));

        return Result.Success;
    }

    public bool CanModify()
    {
        return (DateTimeOffset.UtcNow - CreatedAt).TotalHours < 48;
    }

    private static Result<Success> ValidateRating(decimal rating)
    {
        if (rating < 1m || rating > 5m)
        {
            return Error.Validation("Review.InvalidRating", "Rating must be between 1 and 5.");
        }

        if (rating % 0.5m != 0)
        {
            return Error.Validation("Review.InvalidRatingIncrement", "Rating must be in increments of 0.5.");
        }

        return Result.Success;
    }

    private static Result<Success> ValidateComment(string? comment)
    {
        if (comment != null)
        {
            if (comment.Length < 10)
            {
                return Error.Validation("Review.CommentTooShort", "Comment must be at least 10 characters.");
            }

            if (comment.Length > 1000)
            {
                return Error.Validation("Review.CommentTooLong", "Comment cannot exceed 1000 characters.");
            }
        }

        return Result.Success;
    }
}
