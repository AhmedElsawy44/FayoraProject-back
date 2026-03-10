using Fayora.Domain.Entitties.Tourist;

namespace Fayora.Application.Features.Tourist.Queries.GetInterests;

public record GetInterestsResult(IEnumerable<MasterInterest> Interests);
