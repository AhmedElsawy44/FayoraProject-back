using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccountWithEmail;


public record RestoreAccountWithEmailCommand(
    string Email,
    string Code,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage
) : IRequest<Result<RestoreAccountWithEmailResult>>, ICheckBannedRequest;

