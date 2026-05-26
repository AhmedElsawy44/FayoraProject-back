using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Features.AdminModule.Queries.GetTourCompanyVerificationDetails;
using Fayora.Application.Features.AdminModule.Queries.GetVerificationQueue;
using Fayora.Contracts.AdminModule.GetVerificationQueue;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class TourCompanyRepository(ApplicationDbContext context) : ITourCompanyRepository
{
    public void AddTourCompany(TourCompany tourCompany)
    {
        context.TourCompanies.Add(tourCompany);
    }

    public async Task<TourCompany?> GetTourCompanyByIdAsync(
        Guid id,
        ITourGuideRepository.GuideQueryOptions options,
        CancellationToken cancellationToken = default)
    {
        var query = context.TourCompanies.AsQueryable();

        if (options.ReadOnly)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);
    }

    public async Task<int> GetActiveCompaniesCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.TourCompanies
            .CountAsync(x => x.IsAvailableForBooking, cancellationToken);
    }

    public async Task<int> GetCompaniesOnboardingStatsAsync(CancellationToken cancellationToken = default)
    {
        int totalInOnboarding = await context.TourCompanies
            .CountAsync(x => x.Status == ItemStatus.Pending, cancellationToken);

        return totalInOnboarding;
    }

    public Task<bool> TourCompanyExistAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.TourCompanies.AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<List<GetVerificationQueueResoponse>> GetPendingCompaniesForVerificationAsync(CancellationToken cancellationToken)
    {
        return await (from company in context.TourCompanies
                      where company.Status == ItemStatus.Pending
                      orderby company.CreatedAt descending
                      select new GetVerificationQueueResoponse
                      (
                          company.UserId,
                          company.CompanyName,
                          PartnerTypeFilter.Company,
                          1,
                          company.CreatedAt.UtcDateTime
                      ))
                      .ToListAsync(cancellationToken);
    }

    public async Task<GetTourCompanyVerificationDetailsResponse?> GetVerificationDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var companyWithUser = await (from company in context.TourCompanies
                                     join user in context.Users on company.UserId equals user.Id
                                     where company.UserId == id
                                     select new { company, user })
                                     .FirstOrDefaultAsync(cancellationToken);

        if (companyWithUser is null)
        {
            return null;
        }

        var currentCompany = companyWithUser.company;
        var currentUser = companyWithUser.user;

        return new GetTourCompanyVerificationDetailsResponse
        (
            currentCompany.UserId,
            currentUser.FullName,
            currentUser.PrimaryEmail?.Value,
            currentUser.PhoneNumber?.Value,
            currentCompany.CompanyName,
            currentCompany.LicenseClass.ToString(),
            currentCompany.LicenseDocumentUrl.Value
        );
    }
}