namespace Fayora.Application.Features.Auth.Common;

public record AuthResult(Guid Id, string Identifier, string AccessToken, string RefreshToken);