namespace Fayora.Contracts.Tourist;

public record CreateTouristResponse(Guid Id, string AccessToken, string RefreshToken);
