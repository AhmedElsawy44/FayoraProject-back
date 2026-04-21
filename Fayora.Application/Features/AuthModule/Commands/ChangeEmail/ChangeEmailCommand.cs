using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.ChangeEmail;

public record ChangeEmailCommand(string Email, string Password, string DeviceId) : ICommand<Result<Unit>>, ICheckBannedRequest;
