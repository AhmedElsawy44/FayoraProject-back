using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public record ConfirmChangeEmailCommand(
    string DeviceId,
    string NewEmail,
    string Code) : ICheckBannedRequest, ICommand<Result<ConfirmChangeEmailResult>>;