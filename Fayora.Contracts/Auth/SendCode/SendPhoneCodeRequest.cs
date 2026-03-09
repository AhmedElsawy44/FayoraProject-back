namespace Fayora.Contracts.Auth.SendCode;

public record SendPhoneCodeRequest(
    string PhoneNumber,
    string Purpose,
    string DeliveryMethod
);