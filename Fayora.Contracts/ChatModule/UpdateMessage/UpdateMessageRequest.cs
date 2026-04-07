namespace Fayora.Contracts.ChatModule.UpdateMessage;

public record UpdateMessageRequest(Guid MessageId, string NewContent);
