using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.VerificationModule.Queries.GetVerificationRequest;

public record GetVerificationRequestResponse(
    int VerificationRequestId,
    Guid UserId,
    string RequestType,
    RequestStatus Status,
    string? AdminComment,
    DateTimeOffset SubmittedAt,
    DateTimeOffset? ReviewedAt,
    List<DocumentResponseDto> Documents
  );

public record DocumentResponseDto(
    string DocumentType,
    string DocumentUrl,
    DateOnly? ExpireDate);
