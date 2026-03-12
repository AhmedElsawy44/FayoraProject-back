namespace Fayora.Contracts.TouristModule;

public record InterestDto(int Id, string Name, string IconUrl);

public record InterestsResponse(IEnumerable<InterestDto> Interests);
