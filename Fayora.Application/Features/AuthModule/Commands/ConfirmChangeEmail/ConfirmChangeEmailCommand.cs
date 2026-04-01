using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;

public record ConfirmChangeEmailCommand(
    string DeviceId,
    string NewEmail,
    string Code) : ICheckBannedRequest, IRequest<Result<ConfirmChangeEmailResult>>;