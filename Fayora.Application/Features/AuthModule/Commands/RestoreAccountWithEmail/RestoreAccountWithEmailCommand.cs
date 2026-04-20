using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;


public record RestoreAccountWithEmailCommand(
    string Email,
    string Code,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage
) : ICommand<Result<RestoreAccountWithEmailResult>>, ICheckBannedRequest;

