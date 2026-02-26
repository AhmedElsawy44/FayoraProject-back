using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokensRepository
{
    public async Task AddTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetTokenAsync(string Token, CancellationToken cancellationToken, bool isTracking)
    {
        if (isTracking)
            return await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == Token, cancellationToken);
        else
            return await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Token == Token, cancellationToken);
    }
}
