namespace Fayora.Contracts.Auth;

public record RegisterResponse(Guid Id, string Identifier, string Provider, bool IsVerified = false);

public enum IdentifierType
{
    Email,
    Phone
}
