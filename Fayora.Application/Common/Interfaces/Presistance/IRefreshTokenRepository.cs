using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IRefreshTokensRepository
{
    Task AddTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetTokenAsync(string Token);
}