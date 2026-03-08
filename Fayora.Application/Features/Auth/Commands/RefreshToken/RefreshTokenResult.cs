using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenResult(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
