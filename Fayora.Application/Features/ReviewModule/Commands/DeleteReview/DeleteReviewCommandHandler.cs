using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.ReviewModule.Commands.DeleteReview;

public class DeleteReviewCommandHandler(
    IReviewRepository reviewRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteReviewCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null || review.IsDeleted)
        {
            return ReviewErrors.ReviewNotFound;
        }

        if (review.ReviewerId != userId)
        {
            return ReviewErrors.NotReviewOwner;
        }

        var deleteResult = review.Delete();
        if (deleteResult.IsError)
        {
            return deleteResult.Errors;
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
