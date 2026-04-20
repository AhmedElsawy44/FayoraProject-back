using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public record LoginWithEmailCommand(
    string Email,
    string Password,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage) : ICommand<Result<LoginWithEmailResult>>, ICheckBannedRequest;
