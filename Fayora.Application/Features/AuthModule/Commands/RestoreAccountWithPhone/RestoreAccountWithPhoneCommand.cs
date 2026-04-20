using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone;

public record RestoreAccountWithPhoneCommand(
    string PhoneNumber,
    string Code,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage
) : ICommand<Result<RestoreAccountWithPhoneResult>>, ICheckBannedRequest;
