using Fayora.Domain.Common.Results;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using MediatR;

namespace Fayora.Application.Features.TourCompanyModule.Commands.DeleteCompanyPackage
{
    public class DeleteCompanyPackageCommandHandler(
        ITourCompanyRepository tourCompanyRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteCompanyPackageCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            DeleteCompanyPackageCommand command,
            CancellationToken cancellationToken)
        {
            // get the package
            var package = await tourCompanyRepository.GetPackageByIdAsync(command.PackageId, cancellationToken);

            if (package is null)
                return Error.NotFound(
                    code: "Package.NotFound",
                    description: "Package not found.");

            // ensure that there are no bookings for this package
            var hasBookings = await tourCompanyRepository.HasAnyBookingsAsync(command.PackageId, cancellationToken);

            if (hasBookings)
                return Error.Conflict(
                    code: "Package.HasBookings",
                    description: "Cannot delete a package that has bookings.");

            // remove the package
            tourCompanyRepository.DeletePackage(package);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
