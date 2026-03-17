namespace Fayora.Contracts.AuthModule.SendCode;

public record SendPhoneCodeRequest(
    string PhoneNumber,
    string Purpose,
    string DeliveryMethod
);