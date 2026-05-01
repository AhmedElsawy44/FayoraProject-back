using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;

public record LoginWithPhoneCommand(
    string PhoneNumber,
    string Password,
    string DeviceId,
    string FcmToken,
    Language DeviceLanguage) : ICommand<Result<LoginWithPhoneResult>>, ICheckBannedRequest;
