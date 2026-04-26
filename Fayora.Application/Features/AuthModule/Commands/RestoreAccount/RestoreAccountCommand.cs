using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccount;


public record RestoreAccountCommand(
    string Code,
    string Value,
    CodeDeliveryMethod Type,
    string DeviceId,
    string FcmToken,
    Language DeviceLanguage
) : ICommand<Result<RestoreAccountResult>>, ICheckBannedRequest;

