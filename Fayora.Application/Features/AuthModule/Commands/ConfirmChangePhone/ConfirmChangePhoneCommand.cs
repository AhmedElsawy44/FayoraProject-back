using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public record ConfirmChangePhoneCommand(
    string DeviceId,
    string NewPhoneNumber,
    string Code) : ICheckBannedRequest, IRequest<Result<ConfirmChangePhoneResult>>;