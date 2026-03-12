using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Verification.Commands.ReviewVerificationRequest
{
    public record ReviewVerificationRequestCommand(
        int RequestId,
        Guid AdminId,
        RequestStatus NewStatus,
        string? AdminComment) : IRequest<Result<ReviewVerificationRequestResponse>>;
}
