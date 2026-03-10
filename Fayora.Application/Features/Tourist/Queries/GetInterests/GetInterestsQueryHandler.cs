using Fayora.Application.Common.Interfaces.Presistances.TouristModule;
using MediatR;

namespace Fayora.Application.Features.Tourist.Queries.GetInterests
{
    public class GetInterestsQueryHandler(IMasterInterestRepository masterInterestRepository) : IRequestHandler<GetInterestsQuery, GetInterestsResult>
    {
        public async Task<GetInterestsResult> Handle(GetInterestsQuery request, CancellationToken cancellationToken)
        {
            var interests = await masterInterestRepository.GetAllInterestsAsync();
            return new GetInterestsResult(interests);
        }
    }
}
