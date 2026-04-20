using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest
{
    public class ReviewVerificationRequestCommandHandler(
        IVerificationRepository verificationRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<ReviewVerificationRequestCommand, Result<ReviewVerificationRequestResponse>>
    {
        public async Task<Result<ReviewVerificationRequestResponse>> Handle(
            ReviewVerificationRequestCommand command,
            CancellationToken ct)
        {
            // Get The Request
            var request = await verificationRepository.GetByIdAsync(command.RequestId, ct);

            if (request is null)
                return Error.NotFound(
                    code: "Verification.NotFound",
                    description: $"Verification request {command.RequestId} not found.");

            // Check if it's still pending
            if (request.RequestStatus != RequestStatus.Pending)
                return Error.Conflict(
                    code: "Verification.AlreadyReviewed",
                    description: "This request has already been reviewed.");

            //check if the new status is either Approved or Rejected 
            if (command.NewStatus != RequestStatus.Approved &&
                command.NewStatus != RequestStatus.Rejected)
                return Error.Validation(
                    code: "Verification.InvalidStatus",
                    description: "Status must be either Approved or Rejected.");

            //Review the request
            request.ReviewRequest(command.AdminId, command.NewStatus, command.AdminComment);

            await unitOfWork.CommitChangesAsync(ct);

            return new ReviewVerificationRequestResponse(
                VerificationRequestId: request.Id,
                Status: request.RequestStatus,
                AdminComment: request.AdminComment,
                ReviewedAt: request.ReviewedAt!.Value);
        }
    }
}
