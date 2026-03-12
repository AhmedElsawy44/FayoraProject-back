using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest
{
    public record ReviewVerificationRequestResponse(
        int VerificationRequestId,
        RequestStatus Status,
        string? AdminComment,
        DateTimeOffset ReviewedAt);
}
