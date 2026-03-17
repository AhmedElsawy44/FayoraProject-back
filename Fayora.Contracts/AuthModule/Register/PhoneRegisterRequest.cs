namespace Fayora.Contracts.AuthModule.Register;

public record PhoneRegisterRequest(
    string PhoneNumber,
    string Password,
    string DeliveryMethod
);

