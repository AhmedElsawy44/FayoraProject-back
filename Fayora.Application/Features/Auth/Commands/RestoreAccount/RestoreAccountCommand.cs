using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccount
{
    // افسم الكاوماند ل 2 واحد لل فون و واحد للايميل
    public record RestoreAccountCommand(
        string Identifier,
        string Code,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    ) : IRequest<Result<AuthResult>>, ICheckBannedRequest
    {
        public string? Email => Identifier.Contains('@') ? Identifier : null;
        public string? PhoneNumber => !Identifier.Contains('@') ? Identifier : null;
        public bool IsEmail => Identifier.Contains('@');
    }
}
