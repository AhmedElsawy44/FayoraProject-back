using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest
{
    public record SubmitVerificationRequestResponse(
        int VerificationRequestId,
        string RequestType,
        RequestStatus Status,
        string Message,
        DateTimeOffset SubmittedAt
        // List<DocumentResponseDto> Documents
        );

    //public record DocumentResponseDto(
    //    string DocumentType,
    //    RequestStatus Status);
}
