using Fayora.Domain.Common.Interfaces;

namespace Fayora.Domain.Common.Events;

public record RevokeRefreshTokensForDeviceEvent(Guid UserId, string DeviceId) : IDomainEvent;
