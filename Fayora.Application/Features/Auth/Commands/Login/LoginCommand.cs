using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string? Email,
    string? PhoneNumber,
    string Password,
    string DeviceId,      
    string? FcmToken      
) : IRequest<Result<AuthResult>>;

