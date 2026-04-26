using Fayora.Application.Common.Abstractions.Messaging;

namespace Fayora.Application.Features.ChatModule.Queries.GetMessages;

public record GetMessagesQuery(
    Guid ChatId,
    int Limit = 50,
    DateTimeOffset? Cursor = null
) : IQuery<GetMessagesResult>;
