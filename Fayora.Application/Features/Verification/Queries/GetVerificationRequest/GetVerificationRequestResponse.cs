using Fayora.Domain.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Queries.GetVerificationRequest
{
    public record GetVerificationRequestResponse(
        int VerificationRequestId,
        Guid UserId,
        string RequestType,
        RequestStatus Status,
        string? AdminComment,
        DateTimeOffset SubmittedAt,
        DateTimeOffset? ReviewedAt
      //  List<DocumentResponseDto> Documents
      );

    //public record DocumentResponseDto(
    //    string DocumentType,
    //    string DocumentUrl,
    //    RequestStatus Status,
    //    string? RejectionReason,
    //    DateOnly? ExpireDate);
}
