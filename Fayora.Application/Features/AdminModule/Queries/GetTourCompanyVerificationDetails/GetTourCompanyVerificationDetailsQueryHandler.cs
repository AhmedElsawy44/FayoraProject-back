using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetTourCompanyVerificationDetails;

public class GetTourCompanyVerificationDetailsHandler(ITourCompanyRepository tourCompanyRepository)
    : IQueryHandler<GetTourCompanyVerificationDetailsQuery, Result<GetTourCompanyVerificationDetailsResponse>>
{
    public async Task<Result<GetTourCompanyVerificationDetailsResponse>> Handle(
        GetTourCompanyVerificationDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await tourCompanyRepository.GetVerificationDetailsAsync(request.Id, cancellationToken);

        if (response is null)
        {
            return TourGuideErrors.GuideNotFound;
        }

        return response;
    }
}
