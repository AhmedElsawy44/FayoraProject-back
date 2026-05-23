using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.NotificationModule.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand() : ICommand<Result<Success>>;
