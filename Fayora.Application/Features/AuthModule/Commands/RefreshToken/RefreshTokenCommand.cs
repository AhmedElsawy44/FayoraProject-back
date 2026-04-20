using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
namespace Fayora.Application.Features.AuthModule.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string RefreshToken,
        string DeviceId
    ) : ICommand<Result<RefreshTokenResult>>;
}
