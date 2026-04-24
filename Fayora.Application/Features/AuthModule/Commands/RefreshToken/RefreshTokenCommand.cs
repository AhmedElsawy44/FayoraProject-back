using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken,
    string DeviceId
) : ICommand<Result<RefreshTokenResult>>;
