using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Queries.GetVerificationRequest
{
    public class GetVerificationRequestQueryHandler(
        IVerificationRepository verificationRepository)
        : IRequestHandler<GetVerificationRequestQuery, Result<GetVerificationRequestResponse>>
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
                ReviewedAt: request.ReviewedAt,
                Documents: request.VerificationDocuments
                    .Select(d => new DocumentResponseDto(
                        DocumentType: d.DocumentType.ToString(),
                        DocumentUrl: d.DocumentUrl,
                        Status: d.DocumentStatus,
                        RejectionReason: d.RejectionReason,
                        ExpireDate: d.ExpireDate))
                    .ToList());
        }
    }
}
