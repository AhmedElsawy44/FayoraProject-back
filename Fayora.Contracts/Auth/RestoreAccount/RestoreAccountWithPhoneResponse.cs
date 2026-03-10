using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.RestoreAccount
{
    public record RestoreAccountWithPhoneResponse(
        string UserId,
        string PhoneNumber,
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
