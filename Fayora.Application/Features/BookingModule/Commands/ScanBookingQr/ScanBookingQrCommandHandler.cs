using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using System;
using System.Collections.Generic;
using System.Text;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.BookingModule.Commands.ScanBookingQr
{
    public class ScanBookingQrCommandHandler(
        IBookingRepository bookingRepository,
        IQrTokenService qrTokenService,
        IUserRepository userRepository,
        IHousingUnitRepository housingUnitRepository,
        IPackageRepository packageRepository,
        IClientContextProvider clientContextProvider,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<ScanBookingQrCommand, Result<ScanBookingQrResult>>
    {
        public async Task<Result<ScanBookingQrResult>> Handle(
            ScanBookingQrCommand request,
            CancellationToken cancellationToken)
        {
            var scannerId = clientContextProvider.GetContext().UserId;

            // Check token
            var tokenResult = qrTokenService.ValidateToken(request.Token);
            if (tokenResult.IsError) return BookingErrors.InvalidQrToken;

            var payload = tokenResult.Value;

            // check that the scan on qr doing by service provider
            if (payload.ServiceProviderId != scannerId)
                return BookingErrors.UnauthorizedScan;

            var booking = await bookingRepository.GetBookingByIdAsync(
                payload.BookingId, cancellationToken);
            if (booking is null) return BookingErrors.BookingNotFound;


            if (booking.ServiceId != payload.ServiceId)
                return BookingErrors.InvalidQrToken;

            string serviceName = booking.ServiceType switch
            {
                ServiceType.GuidePackage => await packageRepository
                    .GetPackageByIdAsync(booking.ServiceId,
                        new PackageQueryOptions { ReadOnly = true },
                        cancellationToken)
                    .ContinueWith(t => t.Result?.Title ?? "Unknown"),

                ServiceType.Accommodation => await housingUnitRepository
                    .GetUnitByIdAsync(booking.ServiceId,
                        new UnitQueryOptions { IsReadOnly = true },
                        cancellationToken)
                    .ContinueWith(t => t.Result?.Title ?? "Unknown"),

                _ => "Unknown"
            };

            
            if (booking.PaymentStatus != PaymentTransactionStatus.Paid)
                return BookingErrors.BookingNotPaid;

            if (booking.BookingStatus == BookingStatus.Cancelled)
                return BookingErrors.BookingCancelled; // علشان ممكن يكون تم الغاء الحجز بعد ما تم توليد ال QR code (اصحي ياحمد ليغفلونا)

            // One-Time Check
            var scanResult = booking.MarkAsScanned();
            if (scanResult.IsError) return scanResult.Errors;

            var user = await userRepository.GetUserByIdAsync(
                booking.UserId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);
            if (user is null) return AuthErrors.UserNotFound;

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new ScanBookingQrResult(
                true,
                $"{user.FirstName} {user.LastName}",
                serviceName, 
                booking.ServiceType.ToString(),
                booking.StartDate,
                booking.SeatsCount,
                booking.BookingStatus.ToString()
            );
        }
    }
}
