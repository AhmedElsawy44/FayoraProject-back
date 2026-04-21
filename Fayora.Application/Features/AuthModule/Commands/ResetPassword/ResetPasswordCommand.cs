using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
namespace Fayora.Application.Features.AuthModule.Commands.ResetPasswordEmail;

public record ResetPasswordCommand(
    string Value,
    string ResetToken,
    string NewPassword,
    CodeDeliveryMethod Type,
    string DeviceId) : ICommand<Result<Unit>>, ICheckBannedRequest;