using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.ReviewModule;
using Fayora.Domain.Enums.ReviewModule;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.ReviewModule.Commands.ReportReview;

public class ReportReviewCommandHandler(
    IReviewRepository reviewRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<ReportReviewCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        ReportReviewCommand request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null || review.IsDeleted)
        {
            return ReviewErrors.ReviewNotFound;
        }

        var alreadyReported = await reviewRepository.HasReportByUserAsync(request.ReviewId, userId, cancellationToken);
        if (alreadyReported)
        {
            return ReviewErrors.AlreadyReported;
        }

        if (!Enum.TryParse<ReportReason>(request.Reason, true, out var reasonEnum))
        {
            return ReviewErrors.InvalidReportReason;
        }

        var reportResult = ReviewReport.Create(
            request.ReviewId,
            userId,
            reasonEnum,
            request.AdditionalNotes);

        if (reportResult.IsError)
        {
            return reportResult.Errors;
        }

        reviewRepository.AddReport(reportResult.Value);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
