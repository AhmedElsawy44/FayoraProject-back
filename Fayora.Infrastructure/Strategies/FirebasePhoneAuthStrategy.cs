using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;
using FirebaseAdmin.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Infrastructure.Strategies;

public class FirebasePhoneAuthStrategy(IFirebaseNotificationService firebaseNotificationService) : ISocialAuthStrategy
{
    public IdentityProvider IdentityProvider => IdentityProvider.Phone;

    public async Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            // Verify the Firebase ID Token using Firebase Admin SDK
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, cancellationToken);
            if (decodedToken == null) return null;

            // Extract the Uid
            string subjectId = decodedToken.Uid;

            // Extract phone number from token claims
            decodedToken.Claims.TryGetValue("phone_number", out var phoneNumberObj);
            string? phoneNumber = phoneNumberObj?.ToString();

            return new SocialUserInfo(
                SubjectId: subjectId,
                Email: null,
                FirstName: null,
                LastName: null,
                PictureUrl: null,
                PhoneNumber: phoneNumber
            );
        }
        catch (Exception)
        {
            return null;
        }
    }
}
