using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Enums.SharedModule;
using Microsoft.AspNetCore.Http;


namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest
{
    public record SubmitVerificationRequestCommand(
        Guid UserId,
        RequestType RequestType,
        List<(DocumentType DocumentType, IFormFile File)> Documents
    ) : ICommand<Result<SubmitVerificationRequestResponse>>;
}

