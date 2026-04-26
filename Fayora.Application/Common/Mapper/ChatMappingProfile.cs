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
        CreateMap<ChatDto, Chat>();
        CreateMap<GetChatsResult, GetChatsResponse>();

        CreateMap<Domain.Entities.ChatModule.Message, Contracts.ChatModule.GetMessages.Message>()
            .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.CreatedAt));

        // 3. Map Result -> Response
        CreateMap<GetMessagesResult, GetMessagesResponse>();
    }
}
