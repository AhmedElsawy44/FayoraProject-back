using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Application.Features.TourGuideModule.Queries.GetGuidePackageById;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.GetGuidePackageById;

namespace Fayora.Application.Common.Mapper;

public class TourGuideMappingProfile : Profile
{
    public TourGuideMappingProfile()
    {
        CreateMap<CreateTourGuideResult, CreateTourGuideResponse>();
        CreateMap<GetGuidePackageByIdResult, GetGuidePackageByIdResponse>()
            .ForMember(dest => dest.TourType, opt => opt.MapFrom(src => src.TourType.ToString()))
            .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType.ToString()))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.MeetingPoint.Longitude.ToString()))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.MeetingPoint.Latitude.ToString()));
    }
}
