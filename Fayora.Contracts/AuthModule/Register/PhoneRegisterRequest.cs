namespace Fayora.Contracts.AuthModule.Register;

public record PhoneRegisterRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Password,
    string DeliveryMethod
);

