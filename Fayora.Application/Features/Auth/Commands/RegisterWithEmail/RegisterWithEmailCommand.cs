using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.RegisterWithEmail;

public record RegisterWithEmailCommand(
    string Email,
    string Password,
    string DeviceId
    ) : IRequest<Result<RegisterWithEmailResult>>, ICheckBannedRequest;
