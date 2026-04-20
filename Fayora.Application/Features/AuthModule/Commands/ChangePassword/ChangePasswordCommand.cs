using Fayora.Domain.Common.Results;
using MediatR;
using Fayora.Application.Abstractions.Messaging;
namespace Fayora.Application.Features.AuthModule.Commands.ChangePassword;

public record ChangePasswordCommand(string? CurrentPassword, string NewPassword) : ICommand<Result<Unit>>;