using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.VerificationModule.Commands.ReviewVerificationRequest;

public record ReviewVerificationRequestResult(
    int VerificationRequestId,
    RequestStatus Status,
    string? AdminComment,
    DateTimeOffset ReviewedAt);
