using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;
namespace Fayora.Application.Features.AuthModule.Commands.ChangePassword;

public record ChangePasswordCommand(string? CurrentPassword, string NewPassword) : ICommand<Result<Unit>>;