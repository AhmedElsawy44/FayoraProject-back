using Fayora.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Services.AuthServices
{

    // Dummy  بس عشان اعرف اعمل ميجريشن لحد مانت تعمله
    public class GoogleAuthService : IGoogleAuthService
    {
        public Task<IGoogleAuthService.GoogleAuthenticationResult> GetUserInfoAsync(
            string token,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
