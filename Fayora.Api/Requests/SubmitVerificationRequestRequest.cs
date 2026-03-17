using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Api.Requests
{
    public record SubmitVerificationRequestRequest(
        Guid UserId,
        RequestType RequestType,
        List<DocumentRequestDto> Documents);

    public record DocumentRequestDto(
        DocumentType DocumentType,
        IFormFile File);
}
