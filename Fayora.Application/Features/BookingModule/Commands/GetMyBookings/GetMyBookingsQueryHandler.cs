using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Commands.GetMyBookings;

public class GetMyBookingsQueryHandler(
    IClientContextProvider clientContextProvider,
    IBookingRepository bookingRepository,
    IPackageRepository packageRepository,
    IHousingUnitRepository housingUnitRepository
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

        var packages = packageIds.Any()
            ? await packageRepository.GetListByIdsAsync(packageIds, cancellationToken)
            : [];

        var accommodations = accommodationIds.Any()
            ? await housingUnitRepository.GetUnitsByIdsAsync(accommodationIds, cancellationToken)
            : [];

        var packagesDict = packages.ToDictionary(p => p.Id);
        var accommodationsDict = accommodations.ToDictionary(a => a.Id);

        var responseItems = new List<BookingResponse>();

        foreach (var booking in bookings)
        {
            if (booking.ServiceType == ServiceType.GuidePackage)
            {
                if (packagesDict.TryGetValue(booking.ServiceId, out var package))
                {
                    responseItems.Add(new BookingResponse(
                        booking.Id,
                        package.Title,
                        package.MeetingPoint,
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
        }

        return [.. responseItems.OrderByDescending(x => x.Date)];
    }
}