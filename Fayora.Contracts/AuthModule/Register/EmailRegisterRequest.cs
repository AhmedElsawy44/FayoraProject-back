namespace Fayora.Contracts.AuthModule.Register;

public record EmailRegisterRequest(
    string Email,
    string Password
);