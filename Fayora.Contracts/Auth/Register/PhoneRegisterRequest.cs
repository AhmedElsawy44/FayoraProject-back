namespace Fayora.Contracts.Auth.Register;

public record PhoneRegisterRequest(
    string PhoneNumber,
    string Password,
    string DeliveryMethod
);

