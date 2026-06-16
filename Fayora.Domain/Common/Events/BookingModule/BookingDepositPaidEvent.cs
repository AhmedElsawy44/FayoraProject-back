using System;
using Fayora.Domain.Common.Interfaces.IdentityModule;

namespace Fayora.Domain.Common.Events.BookingModule;

public record BookingDepositPaidEvent(Guid BookingId) : IDomainEvent;
