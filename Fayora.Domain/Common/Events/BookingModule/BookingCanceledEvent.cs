using Fayora.Domain.Common.Interfaces.IdentityModule;

namespace Fayora.Domain.Common.Events.BookingModule;

public record BookingCanceledEvent(Guid BookingId) : IDomainEvent;