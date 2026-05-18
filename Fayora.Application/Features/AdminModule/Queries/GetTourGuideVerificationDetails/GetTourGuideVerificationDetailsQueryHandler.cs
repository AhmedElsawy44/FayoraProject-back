using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetTourGuideVerificationDetails;

public class GetTourGuideVerificationDetailsHandler(ITourGuideRepository tourGuideRepository)
    : IQueryHandler<GetTourGuideVerificationDetailsQuery, Result<GetTourGuideVerificationDetailsResponse>>
{
    public async Task<Result<GetTourGuideVerificationDetailsResponse>> Handle(
        GetTourGuideVerificationDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await tourGuideRepository.GetVerificationDetailsAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return TourGuideErrors.GuideNotFound;
        }

        return response;
    }
}
