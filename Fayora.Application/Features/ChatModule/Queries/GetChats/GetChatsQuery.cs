using MediatR;

namespace Fayora.Application.Features.ChatModule.Queries.GetChats;

public record GetChatsQuery(
    int Limit = 20,
    DateTimeOffset? Cursor = null) : IRequest<GetChatsResult>;
