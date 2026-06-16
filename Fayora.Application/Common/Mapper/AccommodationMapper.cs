using AutoMapper;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;
using Fayora.Application.Features.AccommodationModule.Queries.GetAllAmenities;
using Fayora.Application.Features.AccommodationModule.Queries.GetUnitById;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Mapper;

public class AccommodationMapper : Profile
{
    public AccommodationMapper()
    {
        CreateMap<AmenityResponse, Amenity>();
        CreateMap<CreateUnitOwnerOwnerResult, CreateUnitOwnerProfileResponse>();

        CreateMap<GetUnitByIdResult, GetUnitByIdResponse>()
            .ForMember(dest => dest.Type,
                opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Coordinates,
                opt => opt.MapFrom(src => $"{src.Coordinates.Latitude},{src.Coordinates.Longitude}"))
            .ForMember(dest => dest.CheckInTime,
                opt => opt.MapFrom(src => src.CheckInTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.CheckOutTime,
                opt => opt.MapFrom(src => src.CheckOutTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.Amenities,
                opt => opt.MapFrom(src => src.Amenities));

        CreateMap<HousingUnit, GetAllUnitsByTypeResponse>()
            .ConstructUsing(src => new GetAllUnitsByTypeResponse(
                src.Title,
                src.PricePerNight,
                src.PricePerNight,
                src.Rating,
                src.MainImageUrl.Value,
                src.AddressDetails
            ));
    }
}
