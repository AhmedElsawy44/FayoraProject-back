using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Entities.Booking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Infrastructure.Jobs;

public class ProcessAutomaticPayoutsJob(
    IProviderPayoutRepository providerPayoutRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var thresholdDate = DateTime.UtcNow.AddHours(-48);
        var eligibleBookings = await bookingRepository
            .GetEligibleBookingsForAutomaticPayoutAsync(thresholdDate, cancellationToken);

        if (eligibleBookings == null || !eligibleBookings.Any())
        {
            return;
        }

        var bookingsByProvider = eligibleBookings.GroupBy(b => b.ServiceProviderId);

        foreach (var group in bookingsByProvider)
        {
            var providerId = group.Key;
            var bookingsList = group.ToList();

            var totalAmount = bookingsList.Sum(b => b.PayoutAmount);
            if (totalAmount <= 0)
            {
                continue;
            }

            var allBookingsMarkedSuccessfully = true;
            foreach (var booking in bookingsList)
            {
                var markResult = booking.MarkPayoutAsProcessed();
                if (markResult.IsError)
                {
                    allBookingsMarkedSuccessfully = false;
                    break;
                }
            }

            if (!allBookingsMarkedSuccessfully)
            {
                continue;
            }

            var payout = ProviderPayout.Create(providerId, totalAmount);
            providerPayoutRepository.Add(payout);
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }
}
