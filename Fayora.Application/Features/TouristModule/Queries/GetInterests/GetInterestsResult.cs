using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetInterests;

public record GetInterestsResult(IEnumerable<MasterInterest> Interests);
