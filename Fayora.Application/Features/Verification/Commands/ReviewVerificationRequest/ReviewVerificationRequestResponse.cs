using Fayora.Domain.Enums.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Verification.Commands.ReviewVerificationRequest
{
    public record ReviewVerificationRequestResponse(
        int VerificationRequestId,
        RequestStatus Status,
        string? AdminComment,
        DateTimeOffset ReviewedAt);
}
