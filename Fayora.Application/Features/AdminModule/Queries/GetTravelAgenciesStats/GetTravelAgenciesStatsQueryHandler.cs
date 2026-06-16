using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;

namespace Fayora.Application.Features.AdminModule.Queries.GetTravelAgenciesStats;


public class GetTravelAgenciesStatsHandler(
    ITourCompanyRepository tourCompanyRepository,
    IBookingRepository bookingRepository
) : IQueryHandler<GetTravelAgenciesStatsQuery, TravelAgenciesStatsResponse>
{
    public async Task<TravelAgenciesStatsResponse> Handle(
        GetTravelAgenciesStatsQuery request,
        CancellationToken cancellationToken)
    {
        int activeAgencies = await tourCompanyRepository.GetActiveCompaniesCountAsync(cancellationToken);

        CombinedGmvDto combinedGmv = await bookingRepository.GetCompaniesGmvAsync(cancellationToken);

        AvgCommissionDto avgCommission = await bookingRepository.GetCompaniesAvgCommissionAsync(cancellationToken);

        int inOnboarding = await tourCompanyRepository.GetCompaniesOnboardingStatsAsync(cancellationToken);

        var response = new TravelAgenciesStatsResponse(
            activeAgencies,
            combinedGmv,
            avgCommission,
            inOnboarding
        );

        return response;
    }
}
