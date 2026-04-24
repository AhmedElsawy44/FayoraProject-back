using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public record ConfirmChangePhoneCommand(
    string DeviceId,
    string NewPhoneNumber,
    string Code) : ICheckBannedRequest, ICommand<Result<ConfirmChangePhoneResult>>;