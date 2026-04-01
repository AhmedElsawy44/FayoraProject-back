using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.RegisterWithEmail;

public record RegisterWithEmailCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string DeviceId
    ) : IRequest<Result<RegisterWithEmailResult>>, ICheckBannedRequest;
