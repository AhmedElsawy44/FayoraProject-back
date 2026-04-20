using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public record ConfirmChangeEmailCommand(
    string DeviceId,
    string NewEmail,
    string Code) : ICheckBannedRequest, ICommand<Result<ConfirmChangeEmailResult>>;