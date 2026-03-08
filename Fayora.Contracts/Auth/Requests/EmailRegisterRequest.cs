namespace Fayora.Contracts.Auth.Requests;

public record EmailRegisterRequest(
    string Email,
    string Password
);