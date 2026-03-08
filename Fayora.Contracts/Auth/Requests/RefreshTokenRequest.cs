using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.Requests
{
    public record RefreshTokenRequest(
        string RefreshToken
    );
}
