using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string RefreshToken,
        string DeviceId
    ) : IRequest<Result<RefreshTokenResult>>;
}
