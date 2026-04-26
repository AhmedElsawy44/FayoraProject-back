namespace Fayora.Contracts.ChatModule.GetMessages;

public record GetMessagesRequest(int Limit = 50, DateTimeOffset? Cursor = null);
