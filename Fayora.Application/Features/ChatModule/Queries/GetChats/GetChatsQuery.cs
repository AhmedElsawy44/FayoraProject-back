using Fayora.Application.Common.Abstractions.Messaging;

namespace Fayora.Application.Features.ChatModule.Queries.GetChats;

public record GetChatsQuery(
    int Limit = 20,
    DateTimeOffset? Cursor = null) : IQuery<GetChatsResult>;
