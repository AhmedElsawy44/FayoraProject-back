using Fayora.Domain.Enums.Shared;
using System;
using System.Collections.Generic;

namespace Fayora.Application.Features.Auth.Commands.SubmitVerificationRequest
{
    public record SubmitVerificationRequestResponse(
        int VerificationRequestId,
        string RequestType,
        RequestStatus Status,
        string Message,
        DateTimeOffset SubmittedAt,
        List<DocumentResponseDto> Documents);

    public record DocumentResponseDto(
        string DocumentType,
        RequestStatus Status);
}
