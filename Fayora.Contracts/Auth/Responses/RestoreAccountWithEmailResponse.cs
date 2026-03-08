using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.Responses
{

    public record RestoreAccountWithEmailResponse(
        string UserId,
        string Email,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );

}
