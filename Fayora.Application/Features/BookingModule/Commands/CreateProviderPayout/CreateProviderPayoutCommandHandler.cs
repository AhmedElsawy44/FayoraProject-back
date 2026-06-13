using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.BookingModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Booking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.BookingModule.Commands.CreateProviderPayout;

public class CreateProviderPayoutCommandHandler(
    IProviderPayoutRepository providerPayoutRepository,
    IBookingRepository bookingRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProviderPayoutCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateProviderPayoutCommand request,
        CancellationToken cancellationToken)
    {
        var providerId = clientContextProvider.GetContext().UserId;

        var bookings = await bookingRepository.GetUnpaidCompletedBookingsAsync(providerId, cancellationToken);

        if (bookings == null || !bookings.Any())
        {
            return BookingErrors.NoUnpaidCompletedBookings;
        }

        var totalAmount = bookings.Sum(b => b.PayoutAmount);

        if (totalAmount <= 0)
        {
            return Error.Validation("Booking.InvalidPayoutAmount", "The total payout amount must be greater than zero.");
        }

        foreach (var booking in bookings)
        {
            var markResult = booking.MarkPayoutAsProcessed();
            if (markResult.IsError)
            {
                return markResult.Errors;
            }
        }

        var payout = ProviderPayout.Create(providerId, totalAmount);
        providerPayoutRepository.Add(payout);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return payout.Id;
    }
}
