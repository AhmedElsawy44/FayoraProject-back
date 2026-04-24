using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;

namespace Fayora.Application.Features.TouristModule.Queries.GetInterests;

public class GetInterestsQueryHandler(IMasterInterestRepository masterInterestRepository) : IQueryHandler<GetInterestsQuery, GetInterestsResult>
{
    public async Task<GetInterestsResult> Handle(GetInterestsQuery request, CancellationToken cancellationToken)
    {
        var interests = await masterInterestRepository.GetAllInterestsAsync();
        return new GetInterestsResult(interests);
    }
}
