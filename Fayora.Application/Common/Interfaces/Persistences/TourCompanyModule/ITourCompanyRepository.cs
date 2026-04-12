using Fayora.Domain.Entities.TourCompanyModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule
{
    public interface ITourCompanyRepository
    {
        Task AddTourCompanyAsync(TourCompany tourCompany, CancellationToken cancellationToken = default);
        Task<TourCompany?> GetTourCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TourCompany?> GetTourCompanyByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddPackageAsync(CompanyTourPackage package, CancellationToken ct = default);
        Task<List<CompanyTourPackage>> GetCompanyPackagesAsync(Guid companyId, CancellationToken ct = default);
    }

}
