using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TourCompanyModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourCompanyModule.Commands.CreateCompanyPackage
{
    public class CreateCompanyPackageCommandHandler(
        ITourCompanyRepository tourCompanyRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateCompanyPackageCommand, Result<CreateCompanyPackageResult>>
    {
        public async Task<Result<CreateCompanyPackageResult>> Handle(
            CreateCompanyPackageCommand command,
            CancellationToken cancellationToken)
        {
            // ensure the company exists
            var company = await tourCompanyRepository.GetTourCompanyByIdAsync(command.CompanyId, cancellationToken);

            if (company is null)
                return Error.NotFound(
                    code: "TourCompany.NotFound",
                    description: "Tour company not found.");

            // ensure the company is active and listing enabled
            if (!company.IsListingEnabled)
                return Error.Conflict(
                    code: "TourCompany.NotActive",
                    description: "Company must be active to create packages.");

            // create the package
            var packageResult = CompanyTourPackage.Create(
                command.CompanyId,
                command.Title,
                command.Description,
                command.TourTypes,
                command.DurationHours,
                command.StartDate,
                command.EndDate,
                command.DepartureLocation,
                command.MaxCapacity,
                command.AdultPrice,
                command.ChildPrice,
                command.CancellationPolicy,
                command.MainImageUrl,
                command.MainVideoUrl,
                command.GuestRequirements);

            if (packageResult.IsError)
                return packageResult.Errors;

            var package = packageResult.Value;

            // add included and excluded items if provided
            if (command.IncludedItems.Any())
                package.AddIncludedItems(command.IncludedItems);

            if (command.ExcludedItems.Any())
                package.AddExcludedItems(command.ExcludedItems);

            // save the package in db
            company.Packages.ToList(); // Load packages
            await tourCompanyRepository.AddPackageAsync(package, cancellationToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new CreateCompanyPackageResult(
                PackageId: package.Id,
                CompanyId: package.CompanyId,
                Title: package.Title,
                Status: package.IsActive ? "Active" : "Inactive",
                CreatedAt: package.CreatedAt);
        }

    }
}
