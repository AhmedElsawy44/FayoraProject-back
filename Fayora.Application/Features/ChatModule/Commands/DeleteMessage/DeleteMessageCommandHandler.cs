using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ChatModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.ChatModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatModule.Commands.DeleteMessage;

public class DeleteMessageCommandHandler(
    IMessageRepository messageRepository,
    IChatRepository chatRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider
    ) : ICommandHandler<DeleteMessageCommand, Result<DeleteMessageResult>>
{
    public async Task<Result<DeleteMessageResult>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = clientContextProvider.GetContext().UserId;

        var message = await messageRepository.GetMessageByIdAsync(request.MessageId, cancellationToken);

        if (message is null || message.SenderId != currentUserId) return ChatErrors.MessageNotFound;

        var chat = await chatRepository.GetChatByIdAsync(message.ChatId, true, cancellationToken);

        if (chat is null) return ChatErrors.ChatNotFound;

        var receiverId = chat.FirstUserId == message.SenderId ? chat.SecondUserId : chat.FirstUserId;

        message.Delete();

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new DeleteMessageResult(
            receiverId,
            message.Id,
            chat.Id,
            message.DeleteAt ?? DateTimeOffset.UtcNow
            );
    }
}
