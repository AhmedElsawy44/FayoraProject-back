using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.ChatModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.ChatModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatModule.Commands.UpdateMessage;

public class UpdateMessageCommandHandler(
    IMessageRepository messageRepository,
    IChatRepository chatRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : ICommandHandler<UpdateMessageCommand, Result<UpdateMessageResult>>
{
    public async Task<Result<UpdateMessageResult>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var message = await messageRepository.GetMessageByIdAsync(request.MessageId, cancellationToken);

        if (message is null || message.SenderId != userId) return ChatErrors.MessageNotFound;

        var chat = await chatRepository.GetChatByIdAsync(message.ChatId, true, cancellationToken);

        if (chat is null) return ChatErrors.ChatNotFound;

        var receiverId = chat.FirstUserId == userId ? chat.SecondUserId : chat.FirstUserId;

        var updateResult = message.UpdateContent(request.NewContent);

        if (updateResult.IsError) return updateResult.Errors;

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new UpdateMessageResult
        (
            receiverId,
            message.Id,
            message.ChatId,
            message.Content,
            message.UpdatedAt ?? DateTimeOffset.UtcNow
        );
    }
}
