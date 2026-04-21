namespace Fayora.Contracts.AuthModule.ResetPassword;

public record VerifyResetPasswordRequest(
    string Code,
    string Identifier,
    string CodeDeliveryMethod);
