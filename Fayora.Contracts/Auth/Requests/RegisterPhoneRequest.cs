namespace Fayora.Contracts.Auth.Requests;

public record RegisterPhoneRequest(
    string PhoneNumber,
    string Password,
    string DeliveryMethod
);

