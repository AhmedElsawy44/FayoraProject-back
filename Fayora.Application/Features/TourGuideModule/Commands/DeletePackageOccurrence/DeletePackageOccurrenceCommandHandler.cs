using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Commands.DeletePackageOccurrence;

public class DeletePackageOccurrenceCommandHandler(
    IPackageRepository packageRepository,
    IPackageOccurrenceRepository occurrenceRepository,
    IBookingRepository bookingRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeletePackageOccurrenceCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        DeletePackageOccurrenceCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = clientContextProvider.GetContext().UserId;

        var package = await packageRepository.GetPackageByIdAsync(
            request.PackageId,
            new IPackageRepository.PackageQueryOptions { ReadOnly = true },
            cancellationToken);

        if (package is null || package.UserId != currentUserId)
            return TourGuideErrors.PackageNotFound;

        var occurrence = await occurrenceRepository.GetOccurrenceByIdAsync(
            request.OccurrenceId, cancellationToken);

        if (occurrence is null || occurrence.PackageId != request.PackageId)
            return TourGuideErrors.OccurrenceNotFound;

        var hasBookings = await bookingRepository.HasBookingsForOccurrenceAsync(
            request.PackageId, occurrence.Date, cancellationToken);

        if (hasBookings)
            return TourGuideErrors.OccurrenceHasActiveBookings;

        occurrenceRepository.Remove(occurrence);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
