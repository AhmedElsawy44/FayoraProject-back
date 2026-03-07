using Fayora.Application.Common.Interfaces.Services;
using System.Security.Cryptography;
using static Fayora.Application.Common.Interfaces.Services.IMessageGenerator;

namespace Fayora.Infrastructure.Services.AuthServices;

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

            MessagelPurpose.ChangePhone => $"Fayora: Code to change phone number is {code}",

            MessagelPurpose.AccountDeletion => $"Fayora: Account deletion code is {code}. Do NOT share this.",

            MessagelPurpose.ReactivateAccount => $"Fayora: Account reactivation code is {code}",

            _ => $"Fayora code: {code}"
        };
    }

    public string CreateWhatsAppMessage(MessagelPurpose purpose, string code)
    {
        return purpose switch
        {
            MessagelPurpose.Registration => $"Welcome to Fayora! 🌍\nYour registration code is: *{code}*\n\nPlease do not share this code with anyone.",

            MessagelPurpose.ResetPassword => $"Fayora 🔒\nYour password reset code is: *{code}*\n\nDo NOT share this code with anyone.",

            MessagelPurpose.ChangePhone => $"Fayora 📱\nYour code to change your phone number is: *{code}*",

            MessagelPurpose.AccountDeletion => $"Fayora ⚠️\nWe received a request to delete your account.\nYour verification code is: *{code}*\n\nIf you didn't request this, please ignore this message.",

            MessagelPurpose.ReactivateAccount => $"Welcome back to Fayora! ✨\nYour account reactivation code is: *{code}*",

            _ => $"Fayora security code: *{code}*"
        };
    }
}