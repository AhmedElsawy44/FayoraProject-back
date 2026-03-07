namespace Fayora.Contracts.Auth.Requests;

public record SendPhoneCodeRequest(
    string PhoneNumber,
    string Purpose,
    string DeliveryMethod
);