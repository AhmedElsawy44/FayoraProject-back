using Fayora.Application.Common.Interfaces.Validations;
using MediatR;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AuthModule.Commands.ChangeEmail;

public record ChangeEmailCommand(string Email, string Password, string DeviceId) : ICheckBannedRequest, ICommand<Result<Unit>>;
