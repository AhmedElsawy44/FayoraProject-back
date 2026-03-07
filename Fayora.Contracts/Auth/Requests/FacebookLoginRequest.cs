using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth.Requests
{

    public record FacebookLoginRequest(
        string AccessToken,
        string DeviceId,
        string FcmToken,
        string DeviceLanguage
    );
}
