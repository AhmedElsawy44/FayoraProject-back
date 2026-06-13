using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Commands.ConfirmCashReceived
{
    public class ConfirmCashReceivedCommandHandler(
        IBookingRepository bookingRepository,
        IClientContextProvider clientContextProvider,
        IUnitOfWork unitOfWork)
        : ICommandHandler<ConfirmCashReceivedCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            ConfirmCashReceivedCommand request,
            CancellationToken cancellationToken)
        {
            var providerId = clientContextProvider.GetContext().UserId;

            var booking = await bookingRepository.GetBookingByIdAsync(
                request.BookingId, cancellationToken);
            if (booking is null) return BookingErrors.BookingNotFound;
            if (booking.ServiceProviderId != providerId) return BookingErrors.Unauthorized;
            if (!booking.IsCashOnArrival) return BookingErrors.NotCashOnArrival;

            var result = booking.ConfirmCashReceived();
            if (result.IsError) return result.Errors;

            await unitOfWork.CommitChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
