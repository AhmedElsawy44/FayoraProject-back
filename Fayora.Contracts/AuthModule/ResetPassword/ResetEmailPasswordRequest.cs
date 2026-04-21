namespace Fayora.Contracts.AuthModule.ResetPassword;

public record ResetEmailPasswordRequest(
    string Identifier,
    string ResetToken,
    string NewPassword,
    string CodeDeliveryMethod
);
