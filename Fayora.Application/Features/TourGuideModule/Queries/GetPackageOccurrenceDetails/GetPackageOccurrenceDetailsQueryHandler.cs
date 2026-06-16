using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageOccurrenceDetails;

public class GetPackageOccurrenceDetailsQueryHandler(
    IPackageRepository packageRepository,
    IBookingRepository bookingRepository,
    IUserRepository userRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetPackageOccurrenceDetailsQuery, Result<GetPackageOccurrenceDetailsResult>>
{
    public async Task<Result<GetPackageOccurrenceDetailsResult>> Handle(
        GetPackageOccurrenceDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = clientContextProvider.GetContext().UserId;
        if (currentUserId == Guid.Empty)
            return TourGuideErrors.Unauthorized;

        var package = await packageRepository.GetPackageByIdAsync(
            request.PackageId,
            new PackageQueryOptions(ReadOnly: true, IncludeOccurrences: true, IncludeMeetingPoints: true),
            cancellationToken);

        if (package is null)
            return TourGuideErrors.PackageNotFound;

        if (package.UserId != currentUserId)
            return TourGuideErrors.UnauthorizedPackageModification;

        var occurrence = package.Occurrences.FirstOrDefault(o => o.Id == request.OccurrenceId);
        if (occurrence is null)
            return TourGuideErrors.OccurrenceNotFound;

        var allBookings = await bookingRepository.GetBookingsForOccurrenceAsync(
            request.PackageId,
            occurrence.Date,
            cancellationToken);

        var paidBookings = allBookings
            .Where(b =>
                b.PaymentStatus == PaymentTransactionStatus.Paid ||
                b.PaymentStatus == PaymentTransactionStatus.PartiallyPaid)
            .ToList();

        int totalBookedSeats = paidBookings.Sum(b => b.SeatsCount);
        int totalActiveSeats = allBookings.Sum(b => b.SeatsCount);
        int maxCapacity = occurrence.AvailableSeats + totalActiveSeats;
        decimal totalRevenue = paidBookings.Sum(b => b.TotalPrice);

        var userIds = paidBookings.Select(b => b.UserId).Distinct().ToList();
        var users = await userRepository.GetUsersByIdsAsync(
            userIds,
            new UserQueryOptions { IsReadOnly = true },
            cancellationToken);

        var userMap = users.ToDictionary(u => u.Id);

        var attendees = paidBookings.Select(b =>
        {
            userMap.TryGetValue(b.UserId, out var user);
            var selectedMeetingPoint = b.SelectedMeetingPointId.HasValue
                ? package.MeetingPoints.FirstOrDefault(mp => mp.Id == b.SelectedMeetingPointId.Value)
                : null;

            return new OccurrenceAttendeeResult(
                UserId: b.UserId,
                FullName: user is not null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                ProfileImageUrl: user?.ProfileImageUrl?.Value,
                SeatsCount: b.SeatsCount,
                PaymentStatus: b.PaymentStatus.ToString(),
                MeetingPointName: selectedMeetingPoint?.MeetingPointName,
                MeetingPointTime: selectedMeetingPoint?.Time
            );
        }).ToList();

        return new GetPackageOccurrenceDetailsResult(
            PackageName: package.Title,
            MainImageUrl: package.MainImageUrl.Value,
            OccurrenceDate: occurrence.Date,
            TotalBookedSeats: totalBookedSeats,
            AvailableSeats: occurrence.AvailableSeats,
            MaxCapacity: maxCapacity,
            TotalRevenue: totalRevenue,
            Attendees: attendees
        );
    }
}
