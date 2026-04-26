using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Authorization;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatModule.Commands.SendMessage;

[Authorize]
public record SendMessageCommand(
    Guid ReceiverId,
    string Content,
    string MessageType,
    string ScopeType,
    Guid ScopeId,
    Guid? ChatId = null
) : ICommand<Result<SendMessageResult>>;
