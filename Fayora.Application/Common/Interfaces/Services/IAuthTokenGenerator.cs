using Fayora.Domain.Entitties.Identity;

namespace Fayora.Application.Common.Interfaces.Services;

public record AuthTokensDto(string AccessToken, string RefreshToken, int ExpiresIn);

public interface IAuthTokenGenerator
{
    Task<AuthTokensDto> GenerateTokensAsync(User user, string deviceId, CancellationToken cancellationToken = default);
}