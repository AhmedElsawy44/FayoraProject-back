using AutoMapper;
using Fayora.Application.Features.ChatModule.Queries.GetChats;
using Fayora.Application.Features.ChatModule.Queries.GetMessages;
using Fayora.Contracts.ChatModule.GetChats;
using Fayora.Contracts.ChatModule.GetMessages;

namespace Fayora.Application.Common.Mapper;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<GetChatsResult, GetChatsResponse>();

        CreateMap<Domain.Entities.ChatModule.Message, Message>()
            .ConstructUsing(src => new Message(
                src.Id,
                src.ChatId,
                src.SenderId,
                src.Content,
                src.CreatedAt,
                src.ReadAt
            ));

        CreateMap<GetMessagesResult, GetMessagesResponse>();
    }
}
