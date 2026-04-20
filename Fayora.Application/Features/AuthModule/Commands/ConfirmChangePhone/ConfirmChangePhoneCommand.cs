using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public record ConfirmChangePhoneCommand(
    string DeviceId,
    string NewPhoneNumber,
    string Code) : ICheckBannedRequest, ICommand<Result<ConfirmChangePhoneResult>>;