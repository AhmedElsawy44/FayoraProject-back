using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using Fayora.Domain.Entities.TourCompanyModule;
using Fayora.Domain.Enums.SharedModule;
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

        public async Task<(List<CompanyTourPackage> Items, int TotalCount)> GetAllPackagesAsync(
            TourType? tourType,
            decimal? minPrice,
            decimal? maxPrice,
            DateOnly? startDate,
            DateOnly? endDate,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = context.CompanyTourPackages
                .Include(x => x.Company)  
                .Where(x => x.IsActive)
                .AsQueryable();

            if (tourType.HasValue)
                query = query.Where(x => (x.TourTypes & tourType.Value) == tourType.Value);

            if (minPrice.HasValue)
                query = query.Where(x => x.AdultPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.AdultPrice <= maxPrice.Value);

            if (startDate.HasValue)
                query = query.Where(x => x.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.EndDate <= endDate.Value);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<List<CompanyTourPackage>> GetCompanyPackagesAsync(Guid companyId, CancellationToken ct = default)
            => await context.CompanyTourPackages
                .Where(x => x.CompanyId == companyId)
                .ToListAsync(ct);

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
