using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Shared.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

public class UserTokenRepository(ApplicationDbContext context) : IUserTokenRepository
{
    public void AddToken(UserTokens token) => context.UserTokens.Add(token);

    public Task<UserTokens?> GetTokenAsync(Guid userId, TokenType tokenType, string tokenHash, CancellationToken cancellationToken = default, bool isReadOnly = false)
    {
        var query = context.UserTokens.AsQueryable();

        if (!isReadOnly)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(rt =>
            rt.UserId == userId &&
            rt.TokenType == tokenType &&
            rt.HashedToken == tokenHash,
            cancellationToken);
    }

    public Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return context.UserTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(setter => setter.SetProperty(rt => rt.RevokedAt, DateTimeOffset.UtcNow), cancellationToken);
    }

    public Task RevokeTokensForDeviceAsync(Guid userId, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default)
    {
        return context.UserTokens
            .Where(rt => rt.UserId == userId && rt.DeviceId == deviceId && rt.TokenType == tokenType && rt.RevokedAt == null)
            .ExecuteUpdateAsync(setter => setter.SetProperty(rt => rt.RevokedAt, DateTimeOffset.UtcNow), cancellationToken);
    }

    public async Task<UserTokens?> GetTokenByHashAsync(
    string tokenHash,
    string deviceId,
    TokenType tokenType,
    CancellationToken cancellationToken = default)
    {
        return await context.UserTokens
            .FirstOrDefaultAsync(
                t => t.HashedToken == tokenHash &&
                     t.DeviceId == deviceId &&
                     t.TokenType == tokenType,
                cancellationToken);
    }
}