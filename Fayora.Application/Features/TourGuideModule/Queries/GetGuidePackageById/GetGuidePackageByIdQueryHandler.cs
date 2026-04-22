using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetGuidePackageById
{
    public class GetGuidePackageByIdQueryHandler(
        ITourGuidePackageRepository tourGuidePackageRepository
        ) : IRequestHandler<GetGuidePackageByIdQuery, Result<GetGuidePackageByIdResult>>
    {
        public async Task<Result<GetGuidePackageByIdResult>> Handle(GetGuidePackageByIdQuery request, CancellationToken cancellationToken)
        {
            var package = await tourGuidePackageRepository.GetPackageByIdAsync(request.PackageId, true, cancellationToken);

            if (package is null) return TourGuideErrors.PackageNotFound;

            return new GetGuidePackageByIdResult(
                package.Id,
                package.GuideId,
                package.Title,
                package.Description,
                package.TourTypes,
                package.DurationHours,
                package.MeetingPoint,
                package.ArrivalNote,
                package.TransportType,
                package.MaxCapacity,
                package.BookingsCount,
                package.AvailableSpots,
                package.AdultPrice,
                package.ChildPrice,
                package.IsActive,
                package.Views,
                package.MainImageUrl,
                package.MainVideoUrl,
                package.GuestRequirements,
                package.ImageURLs,
                package.IncludedItems,
                package.ExcludedItems
                );
        }
    }
}
