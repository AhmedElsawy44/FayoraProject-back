namespace Fayora.Contracts.Auth;

public record AuthResponse(
    UserDto User,
    string AccessToken,
    string RefreshToken);

public record UserDto(Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? PhoneNumber);
