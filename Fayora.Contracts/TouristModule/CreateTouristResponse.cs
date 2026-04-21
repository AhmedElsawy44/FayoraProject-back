namespace Fayora.Contracts.TouristModule;

public record CreateTouristResponse(Guid TouristId, string AccessToken, string RefreshToken);
