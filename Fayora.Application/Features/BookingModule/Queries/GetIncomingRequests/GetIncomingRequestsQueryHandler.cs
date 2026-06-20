using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.BookingModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.BookingModule.Queries.GetIncomingRequests
{
    public class GetIncomingRequestsQueryHandler(
        IClientContextProvider clientContextProvider,
        IBookingRepository bookingRepository,
        IHousingUnitRepository housingUnitRepository,
        IPackageRepository packageRepository,
        IUserRepository userRepository)
        : IQueryHandler<GetIncomingRequestsQuery, List<IncomingRequestResponse>>
    {
        public async Task<List<IncomingRequestResponse>> Handle(
            GetIncomingRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var providerId = clientContextProvider.GetContext().UserId;

            var bookings = await bookingRepository.GetIncomingBookingsAsync(
                providerId,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            if (!bookings.Any())
                return [];

            // Filter if status is specified
            if (request.Status.HasValue)
            {
                bookings = bookings.Where(b => b.BookingStatus == request.Status.Value).ToList();
            }

            var touristIds = bookings.Select(b => b.UserId).Distinct().ToList();
            var tourists = touristIds.Any()
                ? await userRepository.GetUsersByIdsAsync(touristIds, new IUserRepository.UserQueryOptions { IsReadOnly = true }, cancellationToken)
                : [];
            var touristsDict = tourists.ToDictionary(t => t.Id);

            var accommodationIds = bookings
                .Where(b => b.ServiceType == ServiceType.Accommodation)
                .Select(b => b.ServiceId)
                .Distinct()
                .ToList();
            var accommodations = accommodationIds.Any()
                ? await housingUnitRepository.GetUnitsByIdsAsync(accommodationIds, cancellationToken)
                : [];
            var accommodationsDict = accommodations.ToDictionary(a => a.Id);

            var packageIds = bookings
                .Where(b => b.ServiceType == ServiceType.GuidePackage)
                .Select(b => b.ServiceId)
                .Distinct()
                .ToList();
            var packages = packageIds.Any()
                ? await packageRepository.GetListByIdsAsync(packageIds, cancellationToken)
                : [];
            var packagesDict = packages.ToDictionary(p => p.Id);

            var response = new List<IncomingRequestResponse>();

            foreach (var booking in bookings)
            {
                touristsDict.TryGetValue(booking.UserId, out var tourist);
                var touristName = tourist != null ? $"{tourist.FirstName} {tourist.LastName}".Trim() : "Guest";
                var initials = GetInitials(touristName);

                string serviceTitle = "Booking";
                if (booking.ServiceType == ServiceType.Accommodation && accommodationsDict.TryGetValue(booking.ServiceId, out var acc))
                {
                    serviceTitle = acc.Title;
                }
                else if (booking.ServiceType == ServiceType.GuidePackage && packagesDict.TryGetValue(booking.ServiceId, out var pkg))
                {
                    serviceTitle = pkg.Title;
                }

                int nights = (booking.EndDate - booking.StartDate).Days;
                if (nights <= 0) nights = 1;

                response.Add(new IncomingRequestResponse(
                    booking.Id,
                    touristName,
                    initials,
                    serviceTitle,
                    booking.StartDate,
                    booking.EndDate,
                    nights,
                    booking.SeatsCount,
                    booking.BookingStatus,
                    booking.TotalPrice
                ));
            }

            return response;
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "G";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }
    }
}
