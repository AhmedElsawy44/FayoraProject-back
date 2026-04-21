using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Features.VerificationModule.Common;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest;

public class GetVerificationRequestQueryHandler(
    IVerificationRepository verificationRepository)
    : IQueryHandler<GetVerificationRequestQuery, Result<GetVerificationRequestResponse>>
{
    public async Task<Result<GetVerificationRequestResponse>> Handle(
        GetVerificationRequestQuery query,
        CancellationToken cancellationToken)
    {
        var request = await verificationRepository.GetByIdAsync(query.RequestId, cancellationToken);

        if (request is null)
            return VerificationErrors.VerificationRequestNotFound(query.RequestId);

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
                    ExpireDate: d.ExpireDate))
                .ToList()
                );
    }
}
