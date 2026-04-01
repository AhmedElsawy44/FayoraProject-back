namespace Fayora.Contracts.AuthModule.Register;

public record EmailRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);