namespace Fayora.Contracts.Tourist;

public record CreateTouristResponse(Guid TouristId, string AccessToken, string RefreshToken);
