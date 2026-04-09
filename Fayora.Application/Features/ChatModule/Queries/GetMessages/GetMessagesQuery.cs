using MediatR;

namespace Fayora.Application.Features.ChatModule.Queries.GetMessages;

public record GetMessagesQuery(
    Guid ChatId,
    int Limit = 50,
    DateTimeOffset? Cursor = null
) : IRequest<GetMessagesResult>;
