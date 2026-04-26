namespace Fayora.Contracts.ChatModule.GetChats;

public record GetChatsRequest(
    int Limit = 20,
    DateTimeOffset? Cursor = null);
