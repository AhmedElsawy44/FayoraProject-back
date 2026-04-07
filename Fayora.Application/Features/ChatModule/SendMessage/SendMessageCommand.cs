using Fayora.Application.Common.Authorization;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.ChatModule.SendMessage;

[Authorize]
public record SendMessageCommand(
    Guid ReceiverId,
    string Content,
    string MessageType,
    string ScopeType,
    Guid ScopeId,
    Guid? ChatId = null
) : IRequest<Result<SendMessageResult>>;
