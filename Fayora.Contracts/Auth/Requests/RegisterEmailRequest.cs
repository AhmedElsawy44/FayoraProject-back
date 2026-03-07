namespace Fayora.Contracts.Auth.Requests;

public record RegisterEmailRequest(
    string Email,
    string Password
);