namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IMessageGenerator
{
    string GenerateCode(int length = 6);
    (string Subject, string Body) CreateEmailMessage(MessagePurpose purpose, string? code = null);
    string CreateSmsMessage(MessagePurpose purpose, string code);
    string CreateWhatsAppMessage(MessagePurpose purpose, string code);

    public enum MessagePurpose
    {
        Registration,
        Login,
        ResetPassword,
        ChangePhone,
        ChangeEmail,
        EmailVerified,
        AccountDeletion,
        ReactivateAccount
    }
}
