using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.RestoreAccount
{
    public record RestoreAccountWithEmailRequest(
        string Email,
        string Code,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    );
}
