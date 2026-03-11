namespace Fayora.Contracts.Tourist;

public record InterestDto(int Id, string Name, string IconUrl);

public record InterestsResponse(IEnumerable<InterestDto> Interests);
