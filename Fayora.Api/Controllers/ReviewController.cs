using Fayora.Application.Features.ReviewModule.Commands.CreateReview;
using Fayora.Application.Features.ReviewModule.Commands.DeleteReview;
using Fayora.Application.Features.ReviewModule.Commands.ReportReview;
using Fayora.Application.Features.ReviewModule.Commands.UpdateReview;
using Fayora.Application.Features.ReviewModule.Queries.GetMyReviews;
using Fayora.Application.Features.ReviewModule.Queries.GetReviewsByTarget;
using Fayora.Contracts.ReviewModule.CreateReview;
using Fayora.Contracts.ReviewModule.GetReviews;
using Fayora.Contracts.ReviewModule.ReportReview;
using Fayora.Contracts.ReviewModule.UpdateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class ReviewController(ISender sender) : ApiController
{
    [HttpGet("{targetType}/{targetId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewsByTargetAsync(
        [FromRoute] string targetType,
        [FromRoute] Guid targetId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetReviewsByTargetQuery(targetId, targetType, page, pageSize);
        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            reviews => Ok(new GetReviewsResponse(
                reviews.Select(r => new ReviewResponse(
                    r.Id,
                    r.ReviewerId,
                    r.ReviewerName,
                    r.ReviewerImageUrl,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    r.CanEdit
                )).ToList(),
                page,
                pageSize
            )),
            Problem);
    }

    [HttpGet("my-reviews")]
    [Authorize]
    public async Task<IActionResult> GetMyReviewsAsync(CancellationToken cancellationToken)
    {
        var query = new GetMyReviewsQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            reviews => Ok(reviews.Select(r => new ReviewResponse(
                r.Id,
                r.ReviewerId,
                r.ReviewerName,
                r.ReviewerImageUrl,
                r.Rating,
                r.Comment,
                r.CreatedAt,
                r.CanEdit
            )).ToList()),
            Problem);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReviewAsync(
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand(
            request.BookingId,
            request.Rating,
            request.Comment);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            reviewId => Ok(reviewId),
            Problem);
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateReviewAsync(
        [FromRoute] Guid reviewId,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReviewCommand(
            reviewId,
            request.Rating,
            request.Comment);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReviewAsync(
        [FromRoute] Guid reviewId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteReviewCommand(reviewId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPost("{reviewId:guid}/report")]
    [Authorize]
    public async Task<IActionResult> ReportReviewAsync(
        [FromRoute] Guid reviewId,
        [FromBody] ReportReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReportReviewCommand(
            reviewId,
            request.Reason,
            request.AdditionalNotes);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}
