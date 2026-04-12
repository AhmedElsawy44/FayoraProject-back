using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using Fayora.Domain.Entities.TourCompanyModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.TourCompanyModule
{
    public class TourCompanyRepository(ApplicationDbContext context) : ITourCompanyRepository
    {
        public async Task AddPackageAsync(CompanyTourPackage package, CancellationToken ct = default)
           => await context.CompanyTourPackages.AddAsync(package, ct);

        public async Task AddTourCompanyAsync(TourCompany tourCompany, CancellationToken cancellationToken = default) 
            => await context.TourCompanies.AddAsync(tourCompany, cancellationToken);


        public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
            => await context.TourCompanies.AnyAsync(x => x.UserId == userId, cancellationToken);

        public async Task<TourCompany?> GetTourCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
          return await context.TourCompanies
                .Include(x => x.Packages)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TourCompany?> GetTourCompanyByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.TourCompanies
                .Include(x => x.Packages)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }
    }
}
