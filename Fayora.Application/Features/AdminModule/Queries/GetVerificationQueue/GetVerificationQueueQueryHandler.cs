using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Contracts.AdminModule.GetVerificationQueue;

namespace Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;

public class GetVerificationQueueHandler(
    ITourCompanyRepository tourCompanyRepository,
    ITourGuideRepository tourGuideRepository
) : IQueryHandler<GetVerificationQueueQuery, List<GetVerificationQueueResoponse>>
{
    public async Task<List<GetVerificationQueueResoponse>> Handle(
        GetVerificationQueueQuery request,
        CancellationToken cancellationToken)
    {
        var combinedList = new List<GetVerificationQueueResoponse>();


        if (request.Type == PartnerTypeFilter.All || request.Type == PartnerTypeFilter.Company)
        {
            var pendingCompanies = await tourCompanyRepository.GetPendingCompaniesForVerificationAsync(cancellationToken);

            combinedList.AddRange(pendingCompanies);
        }

        if (request.Type == PartnerTypeFilter.Guide || request.Type == PartnerTypeFilter.All)
        {
            var pendingGuides = await tourGuideRepository.GetPendingGuidesForVerificationAsync(cancellationToken);

            combinedList.AddRange(pendingGuides);
        }

        var paginatedResult = combinedList
            .OrderByDescending(x => x.SubmittedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paginatedResult;
    }
}