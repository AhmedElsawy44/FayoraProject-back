using Fayora.Domain.Enums.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

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
