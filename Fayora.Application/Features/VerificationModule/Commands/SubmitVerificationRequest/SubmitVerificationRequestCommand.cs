using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest
{
    public record SubmitVerificationRequestCommand(
        Guid UserId,
        RequestType RequestType,
        List<(DocumentType DocumentType, IFormFile File)> Documents
    ) : IRequest<Result<SubmitVerificationRequestResponse>>;
}

