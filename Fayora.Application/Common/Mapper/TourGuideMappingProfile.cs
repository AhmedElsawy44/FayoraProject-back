using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.GetTourGuideById;
using Fayora.Domain.Entities.SharedModule;

namespace Fayora.Application.Common.Mapper;

public class TourGuideMappingProfile : Profile
{
    public TourGuideMappingProfile()
    {
        CreateMap<CreateTourGuideResult, CreateTourGuideResponse>();
        //CreateMap<GetGuidePackageByIdResult, GetGuidePackageByIdResponse>()
        //    .ForMember(dest => dest.TourType, opt => opt.MapFrom(src => src.TourType.ToString()))
        //    .ForMember(dest => dest.TransportType, opt => opt.MapFrom(src => src.TransportType.ToString()))
        //    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.MeetingPoint.Longitude.ToString()))
        //    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.MeetingPoint.Latitude.ToString()));

        CreateMap<City, CityResponse>()
        .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.CenterCoordinates.Latitude))
        .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.CenterCoordinates.Longitude));

        //CreateMap<GuideTourPackageSummaryDto, GuideTourPackageSummaryResponse>()
        //    .ForMember(dest => dest.TourType, opt => opt.MapFrom(src => src.TourType.ToString()));

        //CreateMap<GetTourGuideByIdResult, GetTourGuideByIdResponse>()
        //    .ForMember(dest => dest.Cities, opt => opt.MapFrom(src => src.GuideCities))
        //    .ForMember(dest => dest.TourPackages, opt => opt.MapFrom(src => src.GuideTourPackages))
        //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
        //    .ForMember(dest => dest.TransportInfo, opt => opt.MapFrom(src => src.TransportInfo != null ? src.TransportInfo.ToString() : null));
    }
}
