using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccountWithEmail
{
    public record RestoreAccountWithEmailResult(
        Guid UserId,
        string Email,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );

}
