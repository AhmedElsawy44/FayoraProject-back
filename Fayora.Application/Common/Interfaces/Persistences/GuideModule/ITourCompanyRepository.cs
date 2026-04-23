using Fayora.Domain.Entities.TourGuideModule;
using static Fayora.Application.Common.Interfaces.Persistences.TourGuideModule.ITourGuideRepository;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourCompanyRepository
{
    Task AddTourCompanyAsync(TourCompany tourCompany, CancellationToken cancellationToken = default);
    Task<TourCompany?> GetTourCompanyByIdAsync(Guid id, GuideQueryOptions options, CancellationToken cancellationToken = default);
    Task<TourCompany?> GetTourCompanyByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> TourCompanyExistAsync(Guid userId, CancellationToken cancellationToken = default);
}
