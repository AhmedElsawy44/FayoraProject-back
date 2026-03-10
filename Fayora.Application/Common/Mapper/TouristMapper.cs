using AutoMapper;
using Fayora.Application.Features.Tourist.Queries.GetInterests;
using Fayora.Contracts.Tourist;
using Fayora.Domain.Entitties.Tourist;

namespace Fayora.Application.Common.Mapper;

public class TouristMapper : Profile
{
    public TouristMapper()
    {
        CreateMap<MasterInterest, InterestDto>();
        CreateMap<GetInterestsResult, InterestsResponse>()
            .ForMember(dest => dest.Interests,
               opt => opt.MapFrom(src => src.Interests));
    }
}
