using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatModule.Commands.MarkChatAsRead;

public record MarkChatAsReadCommand(Guid ChatId) : ICommand<Result<MarkChatAsReadResult>>;