using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Services.BookingModule
{
    public interface IQrTokenService
    {
        string GenerateToken(QrTokenPayload payload);
        Result<QrTokenPayload> ValidateToken(string token);
    }

    public record QrTokenPayload(
        Guid BookingId,
        Guid UserId,
        Guid ServiceProviderId,
        Guid ServiceId,
        DateTime ExpiresAt
    );
}
