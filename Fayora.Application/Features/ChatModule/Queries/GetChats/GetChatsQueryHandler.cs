using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.ChatModule.Queries.GetChats;

public class GetChatsQueryHandler(
    IChatRepository chatRepository,
    IUserRepository userRepository,
    IClientContextProvider clientContextProvider
    ) : IQueryHandler<GetChatsQuery, GetChatsResult>
{
    public async Task<GetChatsResult> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var chats = await chatRepository.GetUserChatsPagedAsync(
            userId,
            request.Limit,
            request.Cursor,
            cancellationToken);

        if (!chats.Any()) return new GetChatsResult([]);

        var otherUserIds = chats.Select(c => c.OtherUserId).Distinct().ToList();

        var usersList = await userRepository.GetUsersByIdsAsync(otherUserIds, new UserQueryOptions { IsReadOnly = true }, cancellationToken);

        var usersDict = usersList.ToDictionary(u => u.Id);

        var enrichedChats = chats.Select(chat =>
        {
            usersDict.TryGetValue(chat.OtherUserId, out var otherUser);

            return new ChatDto(
                chat.ChatId,
                chat.OtherUserId,
                otherUser?.FullName ?? "Unknown User",
                otherUser?.ProfileImageUrl?.Value,
                chat.LastMessageContent,
                chat.LastMessageTime,
                chat.UnreadCount
            );
        }).ToList();

        return new GetChatsResult(enrichedChats);
    }
}