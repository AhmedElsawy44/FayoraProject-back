using Fayora.Contracts.ChatModule.GetChats;

namespace Fayora.Application.Features.ChatModule.Queries.GetChats;

public record GetChatsResult(IEnumerable<ChatDto> Chats);
