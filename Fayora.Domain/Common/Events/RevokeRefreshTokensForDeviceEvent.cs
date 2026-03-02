using Fayora.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Common.Events;

public record RevokeRefreshTokensForDeviceEvent(Guid UserId, string DeviceId) : IDomainEvent;
