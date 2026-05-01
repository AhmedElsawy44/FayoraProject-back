using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public record LoginWithEmailCommand(
    string Email,
    string Password,
    string DeviceId,
    string FcmToken,
    Language DeviceLanguage) : ICommand<Result<LoginWithEmailResult>>, ICheckBannedRequest;
