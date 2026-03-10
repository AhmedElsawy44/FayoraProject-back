namespace Fayora.Contracts.Tourist;

public record InterestDto(int Id, string Name, string IconUrl, int SortOrder);

public record InterestsResponse(IEnumerable<InterestDto> Interests);
