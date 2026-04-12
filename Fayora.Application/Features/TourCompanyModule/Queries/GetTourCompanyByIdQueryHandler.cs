using Fayora.Domain.Common.Results;
using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Queries
{
    public class GetTourCompanyByIdQueryHandler(
        ITourCompanyRepository tourCompanyRepository)
        : IRequestHandler<GetTourCompanyByIdQuery, Result<GetTourCompanyByIdResult>>
    {
        public async Task<Result<GetTourCompanyByIdResult>> Handle(
            GetTourCompanyByIdQuery query,
            CancellationToken cancellationToken)
        {
            var company = await tourCompanyRepository.GetTourCompanyByIdAsync(query.CompanyId, cancellationToken);

            if (company is null)
                return Error.NotFound(
                    code: "TourCompany.NotFound",
                    description: $"Tour company with id {query.CompanyId} not found.");

            return new GetTourCompanyByIdResult(
                CompanyId: company.Id,
                UserId: company.UserId,
                CompanyName: company.CompanyName,
                Description: company.Description,
                CommercialRegisterNumber: company.CommercialRegisterNumber,
                TaxRegistrationNumber: company.TaxRegistrationNumber,
                CurrencyCode: company.CurrencyCode,
                LogoUrl: company.LogoUrl,
                Rating: company.Rating,
                ReviewCount: company.ReviewCount,
                CompletedToursCount: company.CompletedToursCount,
                Status: company.Status.ToString(),
                IsListingEnabled: company.IsListingEnabled,
                CreatedAt: company.CreatedAt);
        }
    }
}
