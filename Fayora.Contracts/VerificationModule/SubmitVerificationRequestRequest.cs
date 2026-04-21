namespace Fayora.Contracts.VerificationModule;

public record SubmitVerificationRequestRequest(
    string RequestType,
    List<DocumentRequestDto> Documents);

public record DocumentRequestDto(
    string DocumentType,
    string FileUrl);
