using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Commands.GenerateBookingQr
{
    public class GenerateBookingQrCommandHandler(
        IBookingRepository bookingRepository,
        IQrTokenService qrTokenService,
        IClientContextProvider clientContextProvider
    ) : ICommandHandler<GenerateBookingQrCommand, Result<GenerateBookingQrResult>>
    {
        public async Task<Result<GenerateBookingQrResult>> Handle(
            GenerateBookingQrCommand request,
            CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var booking = await bookingRepository.GetBookingByIdAsync(
                request.BookingId, cancellationToken);

            if (booking is null) return BookingErrors.BookingNotFound;


            if (booking.UserId != userId)
                return BookingErrors.Unauthorized;


            if (booking.PaymentStatus != PaymentTransactionStatus.Paid)
                return BookingErrors.BookingNotPaid;

            if (booking.BookingStatus == BookingStatus.Cancelled)
                return BookingErrors.BookingCancelled;

            var token = qrTokenService.GenerateToken(new QrTokenPayload(
                booking.Id,
                booking.UserId,
                booking.ServiceProviderId,
                booking.ServiceId,
                booking.EndDate
            ));

            return new GenerateBookingQrResult(token);
        }
    }
}
