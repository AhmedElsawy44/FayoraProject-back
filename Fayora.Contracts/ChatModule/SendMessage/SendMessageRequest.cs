namespace Fayora.Contracts.ChatModule.SendMessage;

public record SendMessageRequest(
    Guid ReceiverId,
    string Content,
    string MessageType,
    string ScopeType,
    Guid ScopeId,
    Guid? ChatId
);
