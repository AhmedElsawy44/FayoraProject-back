using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public record AuthTokensDto(string AccessToken, string RefreshToken, int ExpiresIn);

public interface IAuthTokenGenerator
{
    Task<AuthTokensDto> GenerateTokensAsync(
        User user,
        string deviceId,
        CancellationToken cancellationToken = default);
}