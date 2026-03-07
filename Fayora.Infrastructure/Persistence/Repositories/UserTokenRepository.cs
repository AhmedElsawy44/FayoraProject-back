using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserTokenRepository(ApplicationDbContext context) : IUserTokenRepository
{
    public void AddToken(UserTokens token) => context.UserTokens.Add(token);

    public Task<UserTokens?> GetTokenAsync(string token, CancellationToken cancellationToken, bool isTracking)
    {
        var query = context.UserTokens.AsQueryable();

        if (!isTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(rt => rt.HashedToken == token, cancellationToken);
    }

    public Task<UserTokens?> GetTokenAsync(Guid userId, TokenType tokenType, string tokenHash, CancellationToken cancellationToken = default, bool isTracking = false)
    {
        var query = context.UserTokens.AsQueryable();

        if (!isTracking)
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

    public Task RevokeTokensForDeviceAsync(Guid id, string deviceId, CancellationToken cancellationToken)
    {
        return context.UserTokens
            .Where(rt => rt.UserId == id && rt.DeviceId == deviceId && rt.RevokedAt == null)
            .ExecuteUpdateAsync(setter => setter.SetProperty(rt => rt.RevokedAt, DateTimeOffset.UtcNow), cancellationToken);
    }

    public Task RevokeTokensForDeviceAsync(Guid userId, string deviceId, TokenType tokenType, CancellationToken cancellationToken = default)
    {
        return context.UserTokens
            .Where(rt => rt.UserId == userId && rt.DeviceId == deviceId && rt.TokenType == tokenType && rt.RevokedAt == null)
            .ExecuteUpdateAsync(setter => setter.SetProperty(rt => rt.RevokedAt, DateTimeOffset.UtcNow), cancellationToken);
    }
}