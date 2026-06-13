using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.ITourGuideRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.BookingModule.Queries.GetBookingDetails
{
    public class GetBookingDetailsQueryHandler(
        IBookingRepository bookingRepository,
        IPackageRepository packageRepository,
        IHousingUnitRepository housingUnitRepository,
        ITourGuideRepository tourGuideRepository,
        IUserRepository userRepository,
        IQrTokenService qrTokenService,
        IClientContextProvider clientContextProvider)
        : IQueryHandler<GetBookingDetailsQuery, Result<GetBookingDetailsResult>>
    {
        public async Task<Result<GetBookingDetailsResult>> Handle(
            GetBookingDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var booking = await bookingRepository.GetBookingByIdAsync(
                request.BookingId, cancellationToken);
            if (booking is null) return BookingErrors.BookingNotFound;
            if (booking.UserId != userId) return BookingErrors.Unauthorized;

            string title = string.Empty;
            string imageUrl = string.Empty;
            string? qrToken = null;

            if (booking.ServiceType == ServiceType.GuidePackage)
            {
                var package = await packageRepository.GetPackageByIdAsync(
                    booking.ServiceId,
                    new PackageQueryOptions(ReadOnly: true),
                    cancellationToken);
                if (package is not null)
                {
                    title = package.Title;
                    imageUrl = package.MainImageUrl.Value;
                }
            }
            else if (booking.ServiceType == ServiceType.Accommodation)
            {
                var unit = await housingUnitRepository.GetUnitByIdAsync(
                    booking.ServiceId,
                    new UnitQueryOptions(IsReadOnly: true),
                    cancellationToken);
                if (unit is not null)
                {
                    title = unit.Title;
                    imageUrl = unit.MainImageUrl.Value;
                }
            }
            else if (booking.ServiceType == ServiceType.TourGuide)
            {
                var guide = await tourGuideRepository.GetGuideByIdAsync(
                    booking.ServiceId,
                    new GuideQueryOptions(ReadOnly: true),
                    cancellationToken);
                if (guide is not null)
                {
                    var user = await userRepository.GetUserByIdAsync(
                        guide.UserId,
                        new UserQueryOptions { IsReadOnly = true },
                        cancellationToken);
                    if (user is not null)
                    {
                        title = $"{user.FirstName} {user.LastName}";
                        imageUrl = user.ProfileImageUrl?.Value ?? string.Empty;
                    }
                }
            }

            bool canGenerateQr = !booking.IsScanned && (booking.IsCashOnArrival
                ? booking.PaymentStatus == PaymentTransactionStatus.PartiallyPaid || booking.PaymentStatus == PaymentTransactionStatus.Paid
                : booking.PaymentStatus == PaymentTransactionStatus.Paid);

            if (canGenerateQr)
            {
                qrToken = qrTokenService.GenerateToken(new QrTokenPayload(
                    booking.Id,
                    booking.UserId,
                    booking.ServiceProviderId,
                    booking.ServiceId,
                    booking.EndDate));
            }

            return new GetBookingDetailsResult(
                booking.Id,
                title,
                imageUrl,
                booking.BasePrice,        
                booking.DiscountAmount,
                booking.TotalPrice,
                booking.SeatsCount,
                booking.AdultsCount,
                booking.ChildrenCount,
                booking.StartDate,
                booking.EndDate,
                booking.BookingStatus,
                booking.ServiceType,
                qrToken);
        }
    }
}
