using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.ReviewModule.Commands.AdminDeleteReview;

public class AdminDeleteReviewCommandHandler(
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AdminDeleteReviewCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        AdminDeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null || review.IsDeleted)
        {
            return ReviewErrors.ReviewNotFound;
        }

        var deleteResult = review.AdminDelete();
        if (deleteResult.IsError)
        {
            return deleteResult.Errors;
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
