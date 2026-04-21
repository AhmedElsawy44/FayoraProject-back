using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;

public record LoginWithPhoneCommand(
    string PhoneNumber,
    string Password,
    string DeviceId,
    string FcmToken,
    Language DeviceLanguage) : ICommand<Result<LoginWithPhoneResult>>, ICheckBannedRequest;
