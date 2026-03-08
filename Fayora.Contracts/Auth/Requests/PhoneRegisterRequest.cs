namespace Fayora.Contracts.Auth.Requests;

public record PhoneRegisterRequest(
    string PhoneNumber,
    string Password,
    string DeliveryMethod
);

