using AutoMapper;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Mapper;

public class AccommodationMapper : Profile
{
    public AccommodationMapper()
    {
        CreateMap<CreateUnitOwnerOwnerResult, CreateUnitOwnerProfileResponse>();
        //CreateMap<GetUnitByIdResult, GetUnitByIdResponse>()
        //    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
        //    .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src => $"{src.Coordinates.Latitude},{src.Coordinates.Longitude}"));
        //CreateMap<GetAllMasterAmenitiesResult, GetAllMasterAmenitiesResponse>();

        CreateMap<HousingUnit, GetAllUnitsByTypeResponse>()
            .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.MainImageUrl.Value));
    }
}
