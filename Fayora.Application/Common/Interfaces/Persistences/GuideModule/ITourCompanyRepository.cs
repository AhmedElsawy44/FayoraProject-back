using Fayora.Domain.Entities.GuideModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface ITourCompanyRepository
{
    void AddTourCompany(TourCompany tourCompany);
    Task<TourCompany?> GetTourCompanyByIdAsync(Guid id, GuideQueryOptions options, CancellationToken cancellationToken = default);
    Task<bool> TourCompanyExistAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetActiveCompaniesCountAsync(CancellationToken cancellationToken);
    Task<int> GetCompaniesOnboardingStatsAsync(CancellationToken cancellationToken);
}
