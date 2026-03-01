namespace Fayora.Application.Common.Authentication;

public record AuthResult(Guid Id, string? Email, string? PhoneNumber, string AccessToken, string RefreshToken);
