using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;


namespace Fayora.Application.Features.Verification.Commands.SubmitVerificationRequest
{
    public record SubmitVerificationRequestCommand(
        Guid UserId,
        RequestType RequestType,
        List<(DocumentType DocumentType, IFormFile File)> Documents
    ) : IRequest<Result<SubmitVerificationRequestResponse>>;
}

