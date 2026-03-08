using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.Requests
{
    public record RestoreAccountWithPhoneRequest(
        string PhoneNumber,
        string Code,
        string FcmToken,
        string DeviceLanguage
    );
}
