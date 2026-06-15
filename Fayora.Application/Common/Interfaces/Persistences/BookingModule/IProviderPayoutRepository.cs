using Fayora.Domain.Entities.Booking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Common.Interfaces.Persistences.BookingModule;

public interface IProviderPayoutRepository
{
    void Add(ProviderPayout payout);
}
