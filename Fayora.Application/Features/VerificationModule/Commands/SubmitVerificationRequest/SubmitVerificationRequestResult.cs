using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest;

public record SubmitVerificationRequestResult
(
    int VerificationRequestId,
    string RequestType,
    RequestStatus Status,
    DateTimeOffset SubmittedAt
);
