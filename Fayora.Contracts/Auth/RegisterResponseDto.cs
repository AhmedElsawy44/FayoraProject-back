namespace Fayora.Contracts.Auth;

public record RegisterResponseDto(Guid Id, string Identifier, string Provider, bool IsVerified = false);

public enum IdentifierType
{
    Email,
    Phone
}
