using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.Responses
{
    public record RefreshTokenResponse(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
