namespace Fayora.Contracts.AuthModule.VerifyDeleteEmailAccount;

public record VerifyDeleteEmailAccountRequest(
    string Code,
    string CodeDeliveryMethod);
