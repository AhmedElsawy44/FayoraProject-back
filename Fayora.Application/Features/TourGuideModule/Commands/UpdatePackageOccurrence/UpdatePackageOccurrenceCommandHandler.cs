using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.TourGuideModule.Commands.UpdatePackageOccurrence;

public class UpdatePackageOccurrenceCommandHandler(
    IPackageRepository packageRepository,
    IPackageOccurrenceRepository occurrenceRepository,
    IBookingRepository bookingRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdatePackageOccurrenceCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        UpdatePackageOccurrenceCommand request,
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

        if (occurrence.Date != request.NewDate)
        {
            var existingOccurrences = await occurrenceRepository.GetOccurrencesByPackageIdAsync(
                request.PackageId, cancellationToken);

            var newEnd = request.NewDate.AddDays(package.NumOfDays);

            var dateConflict = existingOccurrences.Any(o =>
            {
                if (o.Id == request.OccurrenceId) return false;
                var existingEnd = o.Date.AddDays(package.NumOfDays);
                return request.NewDate < existingEnd && o.Date < newEnd;
            });

            if (dateConflict)
                return GuideErrors.PackageOccurrenceOverlap;
        }

        occurrence.Update(request.NewDate, request.NewAvailableSeats);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
