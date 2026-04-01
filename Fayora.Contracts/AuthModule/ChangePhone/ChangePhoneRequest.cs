namespace Fayora.Contracts.AuthModule.ChangePhone;

public record ChangePhoneRequest(
    string Phone,
    string Password,
    string CodeDeliveryMethod);