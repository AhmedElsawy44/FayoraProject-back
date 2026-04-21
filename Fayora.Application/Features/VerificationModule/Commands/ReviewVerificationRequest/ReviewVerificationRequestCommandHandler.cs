using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.VerificationModule.Common;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest;

public class ReviewVerificationRequestCommandHandler(
    IVerificationRepository verificationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider)
    : ICommandHandler<ReviewVerificationRequestCommand, Result<ReviewVerificationRequestResult>>
{
    public async Task<Result<ReviewVerificationRequestResult>> Handle(
        ReviewVerificationRequestCommand command,
        CancellationToken cancellationToken)
    {
        var adminId = clientContextProvider.GetContext().UserId;
        // Get The Request
        var request = await verificationRepository.GetByIdAsync(command.RequestId, cancellationToken);

        if (request is null)
            return VerificationErrors.VerificationRequestNotFound(command.RequestId);

        //Review the request
        request.ReviewRequest(adminId, command.NewStatus, command.AdminComment);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new ReviewVerificationRequestResult(
            VerificationRequestId: request.Id,
            Status: request.RequestStatus,
            AdminComment: request.AdminComment,
            ReviewedAt: request.ReviewedAt!.Value);
    }
}
