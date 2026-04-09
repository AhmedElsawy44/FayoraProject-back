using Fayora.Application.Common.Interfaces.Presistances.ChatModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using MediatR;

namespace Fayora.Application.Features.ChatModule.Queries.GetMessages;

public class GetMessagesQueryHandler(
    IChatRepository chatRepository,
    IMessageRepository messageRepository,
    IClientContextProvider clientContextProvider
) : IRequestHandler<GetMessagesQuery, GetMessagesResult>
{
    public async Task<GetMessagesResult> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var chat = await chatRepository.GetChatByIdAsync(
            request.ChatId,
            true,
            cancellationToken);

        if (chat is null || (chat.FirstUserId != userId && chat.SecondUserId != userId))
        {
            return new GetMessagesResult([]);
        }

        var messages = await messageRepository.GetChatMessagesPagedAsync(
            request.ChatId,
            request.Limit,
            request.Cursor,
            cancellationToken);

        return new GetMessagesResult(messages);
    }
}