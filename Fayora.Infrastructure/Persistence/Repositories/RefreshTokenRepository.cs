using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : BaseRepository<RefreshToken, int>(context), IRefreshTokenRepository
{
    public void AddToken(RefreshToken refreshToken) => Add(refreshToken);

    public Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken, bool isTracking) => GetSingleAsync(t => t.Token == token, cancellationToken);

    public Task RevokeTokensForDeviceAsync(Guid id, string deviceId, CancellationToken cancellationToken)
    {
        return context.RefreshTokens
            .Where(rt => rt.UserId == id && rt.DeviceId == deviceId && rt.RevokedAt == null)
            .ExecuteUpdateAsync(setter => setter.SetProperty(rt => rt.RevokedAt, DateTime.UtcNow), cancellationToken);
    }
}
