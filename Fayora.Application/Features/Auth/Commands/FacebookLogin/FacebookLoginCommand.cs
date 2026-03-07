using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.FacebookLogin
{

    public record FacebookLoginCommand(
    string AccessToken,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage
) : IRequest<Result<AuthResult>>, ICheckBannedRequest
    {
        public string Identity => string.Empty;
        public string? Email => null;
        public string? PhoneNumber => null;
        public bool IsEmail => false;
    }
}
