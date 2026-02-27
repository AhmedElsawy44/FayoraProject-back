using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : BaseRepository<RefreshToken, int>(context), IRefreshTokenRepository
{
    public void AddToken(RefreshToken refreshToken) => Add(refreshToken);

    public Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken, bool isTracking) => GetSingleAsync(t => t.Token == token, cancellationToken);
}
