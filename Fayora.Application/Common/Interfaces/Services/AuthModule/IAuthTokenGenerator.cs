using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public record AuthTokensDto(string AccessToken, string RefreshToken, int ExpiresIn);

public interface IAuthTokenGenerator
{
    Task<AuthTokensDto> GenerateTokensAsync(
        User user,
        string deviceId,
        Guid? touristId = null,
        Guid? tourGuideId = null,
        Guid? ownerId = null,
        CancellationToken cancellationToken = default);
}