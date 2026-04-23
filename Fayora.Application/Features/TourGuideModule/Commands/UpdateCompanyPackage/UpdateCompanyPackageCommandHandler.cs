using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdateCompanyPackage
{

    public class UpdateCompanyPackageCommandHandler(
        ITourCompanyRepository tourCompanyRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateCompanyPackageCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            UpdateCompanyPackageCommand command,
            CancellationToken cancellationToken)
        {
            //// get the package
            //var package = await tourCompanyRepository.GetPackageByIdAsync(command.PackageId, cancellationToken);

            //if (package is null)
            //    return Error.NotFound(
            //        code: "Package.NotFound",
            //        description: "Package not found.");

            //// ensure there are no confirmed bookings
            //var hasConfirmedBookings = await tourCompanyRepository.HasConfirmedBookingsAsync(command.PackageId, cancellationToken);

            //if (hasConfirmedBookings)
            //    return Error.Conflict(
            //        code: "Package.HasConfirmedBookings",
            //        description: "Cannot update a package with confirmed bookings.");

            //// update the package
            //package.UpdateDetails(
            //    command.Title,
            //    command.Description,
            //    command.DurationHours,
            //    command.AdultPrice,
            //    command.ChildPrice,
            //    command.TourTypes);

            //package.UpdateDates(command.StartDate, command.EndDate);
            //package.UpdateDepartureLocation(command.DepartureLocation);

            //if (!string.IsNullOrWhiteSpace(command.MainImageUrl))
            //    package.SetMainImage(command.MainImageUrl);

            //package.ClearIncludedItems();
            //package.AddIncludedItems(command.IncludedItems);

            //package.ClearExcludedItems();
            //package.AddExcludedItems(command.ExcludedItems);

            //// save changes to the database
            //await unitOfWork.CommitChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
