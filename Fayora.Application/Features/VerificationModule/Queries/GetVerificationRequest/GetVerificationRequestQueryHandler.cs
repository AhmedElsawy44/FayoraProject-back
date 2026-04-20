using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest
{
    public class GetVerificationRequestQueryHandler(
        IVerificationRepository verificationRepository)
        : IQueryHandler<GetVerificationRequestQuery, Result<GetVerificationRequestResponse>>
    {
        public async Task<Result<GetVerificationRequestResponse>> Handle(
            GetVerificationRequestQuery query,
            CancellationToken ct)
        {
            var request = await verificationRepository.GetByIdAsync(query.RequestId, ct);

            if (request is null)
                return Error.NotFound(
                    code: "Verification.NotFound",
                    description: $"Verification request with id {query.RequestId} not found.");

            return new GetVerificationRequestResponse(
                VerificationRequestId: request.Id,
                UserId: request.UserId,
                RequestType: request.RequestType.ToString(),
                Status: request.RequestStatus,
                AdminComment: request.AdminComment,
                SubmittedAt: request.CreatedAt,
                ReviewedAt: request.ReviewedAt
                    //Documents: request.VerificationDocuments
                    //    .Select(d => new DocumentResponseDto(
                    //        DocumentType: d.DocumentType.ToString(),
                    //        DocumentUrl: d.DocumentUrl,
                    //        Status: d.DocumentStatus,
                    //        RejectionReason: d.RejectionReason,
                    //        ExpireDate: d.ExpireDate))
                    //    .ToList()
                    );
        }
    }
}
