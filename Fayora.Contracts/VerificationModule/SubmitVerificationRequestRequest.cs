using Microsoft.AspNetCore.Http;

namespace Fayora.Contracts.VerificationModule
{
    public record SubmitVerificationRequestRequest(
        Guid UserId,
        string RequestType,
        List<DocumentRequestDto> Documents);

    public record DocumentRequestDto(
        string DocumentType,
        IFormFile File);
}
