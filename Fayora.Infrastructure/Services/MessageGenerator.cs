using Fayora.Application.Common.Interfaces.Services;
using System.Security.Cryptography;
using static Fayora.Application.Common.Interfaces.Services.IMessageGenerator;

namespace Fayora.Infrastructure.Services;

public class MessageGenerator : IMessageGenerator
{
    public string GenerateCode(int length = 6)
    {
        var random = RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, length));
        return random.ToString($"D{length}");
    }

    public (string Subject, string Body) CreateEmailMessage(MessagelPurpose purpose, string? code = null)
    {
        return purpose switch
        {
            MessagelPurpose.Registration => (
                Subject: "Welcome to Fayora!",
                Body: $"Welcome to Fayora! Your verification code is: {code}"
            ),
            MessagelPurpose.ResetPassword => (
                Subject: "Password Reset Code",
                Body: $"Your password reset code is: {code}. Please do not share this with anyone."
            ),
            MessagelPurpose.ChangePhone => (
                Subject: "Phone Number Change Verification",
                Body: $"You requested to change your phone number. Your verification code is: {code}"
            ),
            MessagelPurpose.ChangeEmail => (
                Subject: "Verify Your New Email",
                Body: $"You requested to change your email address. Your verification code is: {code}"
            ),
            MessagelPurpose.AccountDeletion => (
                Subject: "Account Deletion Request",
                Body: $"We received a request to delete your Fayora account. Your verification code is: {code}. If you didn't request this, contact support immediately."
            ),
            MessagelPurpose.ReactivateAccount => (
                Subject: "Reactivate Your Account",
                Body: $"Welcome back! Your code to reactivate your account is: {code}"
            ),
            MessagelPurpose.EmailVerified => (
                Subject: "Email Verified",
                Body: "Your email address has been successfully verified. Thank you for confirming your contact information!"
            ),
            _ => (
                Subject: "Fayora Security Code",
                Body: $"Your Fayora verification code is: {code}"
            )
        };
    }

    public string CreateSmsMessage(MessagelPurpose purpose, string? code = null)
    {
        return purpose switch
        {
            MessagelPurpose.Registration => $"Fayora: Your registration code is {code}",

            MessagelPurpose.ResetPassword => $"Fayora: Password reset code is {code}. Do NOT share this.",

            MessagelPurpose.Login => "Fayora Security: A new login was detected on your account. If this wasn't you, please secure your account immediately.",

            MessagelPurpose.ChangePhone => $"Fayora: Code to change phone number is {code}",

            MessagelPurpose.ChangeEmail => $"Fayora: Code to verify email change is {code}",

            MessagelPurpose.AccountDeletion => $"Fayora: Account deletion code is {code}. Do NOT share this.",

            MessagelPurpose.ReactivateAccount => $"Fayora: Account reactivation code is {code}",

            _ => $"Fayora code: {code}"
        };
    }
}