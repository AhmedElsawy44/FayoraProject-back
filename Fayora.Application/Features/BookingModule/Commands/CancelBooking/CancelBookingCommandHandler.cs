using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Commands.CancelBooking
{
    public class CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        IClientContextProvider clientContextProvider,
        IUnitOfWork unitOfWork) : ICommandHandler<CancelBookingCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            CancelBookingCommand request,
            CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var booking = await bookingRepository.GetBookingByIdAsync(
                request.BookingId, cancellationToken);
            if (booking is null) return BookingErrors.BookingNotFound;
            if (booking.UserId != userId) return BookingErrors.Unauthorized;

            var cancelResult = booking.Cancel("Cancelled by user");
            if (cancelResult.IsError) return cancelResult.Errors;

            await unitOfWork.CommitChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
