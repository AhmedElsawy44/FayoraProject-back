namespace Fayora.Contracts.Auth.Responses;

public record VerifyPhoneResponse(
    Guid Id,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);

