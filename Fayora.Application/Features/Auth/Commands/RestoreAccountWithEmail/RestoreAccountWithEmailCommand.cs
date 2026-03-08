using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Commands.RestoreAccountWithEmail;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccount
{
  
    public record RestoreAccountWithEmailCommand(
        string Email,
        string Code,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    ) : IRequest<Result<RestoreAccountWithEmailResult>>, ICheckBannedRequest
    {
        public string Identity => Email;
        public string? PhoneNumber => null;
        public bool IsEmail => true;
    }
}
