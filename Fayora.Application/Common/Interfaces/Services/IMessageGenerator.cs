namespace Fayora.Application.Common.Interfaces.Services;

public interface IMessageGenerator
{
    string GenerateCode(int length = 6);
    (string Subject, string Body) CreateEmailMessage(MessagelPurpose purpose, string? code = null);
    string CreateSmsMessage(MessagelPurpose purpose, string code);

    public enum MessagelPurpose
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
