using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.BookingModule.Queries.GetMyBookings;

public class GetMyBookingsQueryHandler(
    IClientContextProvider clientContextProvider,
    IBookingRepository bookingRepository,
    IPackageRepository packageRepository,
    IHousingUnitRepository housingUnitRepository,
    ITourGuideRepository tourGuideRepository,
    IUserRepository userRepository
) : IQueryHandler<GetMyBookingsQuery, List<BookingResponse>>
{
    public async Task<List<BookingResponse>> Handle(
        GetMyBookingsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var bookings = await bookingRepository.GetPagedBookingsByUserIdAsync(
            userId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        if (!bookings.Any())
            return [];


        var packageIds = bookings
            .Where(b => b.ServiceType == ServiceType.GuidePackage)
            .Select(b => b.ServiceId)
            .Distinct()
            .ToList();

        var accommodationIds = bookings
            .Where(b => b.ServiceType == ServiceType.Accommodation)
            .Select(b => b.ServiceId)
            .Distinct()
            .ToList();

        var guideIds = bookings
            .Where(b => b.ServiceType == ServiceType.TourGuide)
            .Select(b => b.ServiceId)
            .Distinct()
            .ToList();

        var packages = packageIds.Any()
            ? await packageRepository.GetListByIdsAsync(packageIds, cancellationToken)
            : [];

        var meetingPoints = packageIds.Any()
            ? await packageRepository.GetMeetingPointsByPackageIdsAsync(packageIds, cancellationToken)
            : [];

        var accommodations = accommodationIds.Any()
            ? await housingUnitRepository.GetUnitsByIdsAsync(accommodationIds, cancellationToken)
            : [];

        var guideUsers = guideIds.Any()
            ? await userRepository.GetUsersByIdsAsync(guideIds, new IUserRepository.UserQueryOptions { IsReadOnly = true }, cancellationToken)
            : [];

        var guides = guideIds.Any()
            ? await tourGuideRepository.GetGuidesByIdsAsync(guideIds, new ITourGuideRepository.GuideQueryOptions(ReadOnly: true), cancellationToken)
            : [];


        var packagesDict = packages.ToDictionary(p => p.Id);
        var accommodationsDict = accommodations.ToDictionary(a => a.Id);
        var guideUsersDict = guideUsers.ToDictionary(u => u.Id);
        var guidesDict = guides.ToDictionary(g => g.UserId);

        var responseItems = new List<BookingResponse>();

        foreach (var booking in bookings)
        {
            if (booking.ServiceType == ServiceType.GuidePackage)
            {
                if (packagesDict.TryGetValue(booking.ServiceId, out var package))
                {
                    var mps = meetingPoints.Where(mp => mp.PackageId == package.Id).ToList();
                    var selectedMp = booking.SelectedMeetingPointId.HasValue
                        ? mps.FirstOrDefault(mp => mp.Id == booking.SelectedMeetingPointId.Value)
                        : null;
                    var location = selectedMp?.MeetingPoint ?? mps.FirstOrDefault()?.MeetingPoint ?? GeoPoint.Create(0, 0).Value;

                    responseItems.Add(new BookingResponse(
                        booking.Id,
                        package.Title,
                        location,
                        booking.StartDate,
                        booking.BookingStatus,
                        package.MainImageUrl.Value));
                }
            }
            else if (booking.ServiceType == ServiceType.Accommodation)
            {
                if (accommodationsDict.TryGetValue(booking.ServiceId, out var acc))
                {
                    responseItems.Add(new BookingResponse(
                        booking.Id,
                        acc.Title,
                        acc.Coordinates,
                        booking.StartDate,
                        booking.BookingStatus,
                        acc.MainImageUrl.Value));
                }
            }
            else if (booking.ServiceType == ServiceType.TourGuide)
            {
                if (guideUsersDict.TryGetValue(booking.ServiceId, out var guideUser))
                {
                    guidesDict.TryGetValue(booking.ServiceId, out var guide);
                    var location = guide?.LastLocation ?? GeoPoint.Create(0, 0).Value;

                    responseItems.Add(new BookingResponse(
                        booking.Id,
                        $"{guideUser.FirstName} {guideUser.LastName}",
                        location,
                        booking.StartDate,
                        booking.BookingStatus,
                        guideUser.ProfileImageUrl?.Value ?? string.Empty));
                }
            }
        }

        return [.. responseItems.OrderByDescending(x => x.Date)];
    }
}