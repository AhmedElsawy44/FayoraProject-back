using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;
using FirebaseAdmin.Auth;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Fayora.Infrastructure.Strategies;

public class FirebasePhoneAuthStrategy(
    IFirebaseNotificationService firebaseNotificationService,
    ILogger<FirebasePhoneAuthStrategy> logger) : ISocialAuthStrategy
{
    public IdentityProvider IdentityProvider => IdentityProvider.Phone;

    public async Task<SocialUserInfo?> LoginWithSocialAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            bool isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);

            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                logger.LogWarning("Firebase Admin SDK is not initialized. Falling back to parsing JWT token directly for development.");
                return ParseTokenWithoutVerification(token);
            }

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
            catch (Exception ex)
            {
                logger.LogError(ex, "Firebase ID Token verification failed.");
                if (isDevelopment)
                {
                    logger.LogWarning("Verification failed, but in Development mode. Falling back to parsing JWT token directly.");
                    return ParseTokenWithoutVerification(token);
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in FirebasePhoneAuthStrategy.");
            return null;
        }
    }

    private SocialUserInfo? ParseTokenWithoutVerification(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                logger.LogWarning("Token is not a valid JWT format.");
                return null;
            }

            var jwtToken = handler.ReadJwtToken(token);
            var subjectId = jwtToken.Subject;
            var phoneNumber = jwtToken.Claims.FirstOrDefault(c => c.Type == "phone_number")?.Value;

            if (string.IsNullOrEmpty(subjectId))
            {
                logger.LogWarning("JWT token is missing the subject (sub) claim.");
                return null;
            }

            return new SocialUserInfo(
                SubjectId: subjectId,
                Email: null,
                FirstName: null,
                LastName: null,
                PictureUrl: null,
                PhoneNumber: phoneNumber
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse JWT token without verification.");
            return null;
        }
    }
}
