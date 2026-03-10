using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.RefreshToken
{
    public record RefreshTokenRequest(
        string RefreshToken
    );
}
