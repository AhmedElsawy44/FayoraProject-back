namespace Fayora.Contracts.Auth.Register;

public record EmailRegisterRequest(
    string Email,
    string Password
);