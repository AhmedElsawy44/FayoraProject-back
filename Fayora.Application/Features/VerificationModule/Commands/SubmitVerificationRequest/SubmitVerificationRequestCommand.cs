using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;


namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest;

public record SubmitVerificationRequestCommand(
    RequestType RequestType,
    List<VerificationDocument> Documents
) : ICommand<Result<SubmitVerificationRequestResult>>;

public record VerificationDocument(
    DocumentType DocumentType,
    string FileUrl
);