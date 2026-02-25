namespace Fayora.Application.Common.Authentication;

public record AuthResult(Guid Id, string FirstName, string LastName, string? Email, string? PhoneNumber, string AccessToken, string RefreshToken);
