using Fayora.Domain.Common.Results;

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
