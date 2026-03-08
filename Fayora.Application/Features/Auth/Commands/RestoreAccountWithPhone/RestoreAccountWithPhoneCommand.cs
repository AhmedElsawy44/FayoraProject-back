using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccountWithPhone
{
    public record RestoreAccountWithPhoneCommand(
        string PhoneNumber,
        string Code,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    ) : IRequest<Result<RestoreAccountWithPhoneResult>>, ICheckBannedRequest
    {
        public string Identity => PhoneNumber;
        public string? Email => null;
        public bool IsEmail => false;
    }
}
