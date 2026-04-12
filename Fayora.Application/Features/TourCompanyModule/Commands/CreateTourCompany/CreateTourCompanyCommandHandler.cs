using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TourCompanyModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using Fayora.Application.Features.TourCompanyModule.CreateTourCompany;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Commands.CreateTourCompany
{

    public class CreateTourCompanyCommandHandler(
        ITourCompanyRepository tourCompanyRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateTourCompanyCommand, Result<CreateTourCompanyResult>>
    {
        public async Task<Result<CreateTourCompanyResult>> Handle(
            CreateTourCompanyCommand command,
            CancellationToken cancellationToken)
        {
            // ensure that the user is not already registered as a tour company before creating a new one
            var exists = await tourCompanyRepository.ExistsAsync(command.UserId, cancellationToken);
            if (exists)
                return Error.Conflict(
                    code: "TourCompany.AlreadyExists",
                    description: "User is already registered as a tour company.");

            // Create the tour company entity using the factory method
            var companyResult = TourCompany.Create(
                command.UserId,
                command.CompanyName,
                command.Description,
                command.CommercialRegisterNumber,
                command.TaxRegistrationNumber,
                command.CurrencyCode);

            if (companyResult.IsError)
                return companyResult.Errors;

            var company = companyResult.Value;


            // If a logo URL is provided, set it on the company entity
            if (!string.IsNullOrWhiteSpace(command.LogoUrl))
                company.SetLogo(command.LogoUrl);

            //save the new company to the database
            await tourCompanyRepository.AddTourCompanyAsync(company, cancellationToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            // Return the result with the new company's details
            return new CreateTourCompanyResult(
                CompanyId: company.Id,
                UserId: company.UserId,
                CompanyName: company.CompanyName,
                Status: company.Status.ToString(),
                Message: "Company registered successfully. Pending admin verification.",
                CreatedAt: company.CreatedAt);
        }
    }
}
