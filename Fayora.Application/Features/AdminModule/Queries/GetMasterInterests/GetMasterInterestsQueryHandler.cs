using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Contracts.AdminModule.MasterInterests;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetMasterInterests;

public class GetMasterInterestsQueryHandler(IMasterInterestRepository masterInterestRepository) : IQueryHandler<GetMasterInterestsQuery, Result<List<GetMasterInterestsResponse>>>
{
    public async Task<Result<List<GetMasterInterestsResponse>>> Handle(GetMasterInterestsQuery request, CancellationToken cancellationToken)
    {
        var interests = await masterInterestRepository.GetMasterInterestsAsync(cancellationToken);
        return interests;
    }
}

