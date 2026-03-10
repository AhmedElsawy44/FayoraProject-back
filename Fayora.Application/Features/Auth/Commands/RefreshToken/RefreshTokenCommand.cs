using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string RefreshToken,
        string DeviceId
    ) : IRequest<Result<RefreshTokenResult>>;
}
