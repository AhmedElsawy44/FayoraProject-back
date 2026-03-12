using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Features.Tourist.Queries.GetInterests;

public record GetInterestsResult(IEnumerable<MasterInterest> Interests);
