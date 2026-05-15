using AutoMapper;
using Fayora.Application.Features.TouristModule.Commands.CreateTouristProfile;
using Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages;
using Fayora.Application.Features.TouristModule.Queries.GetAllLocations;
using Fayora.Application.Features.TouristModule.Queries.GetInterests;
using Fayora.Application.Features.TouristModule.Queries.GetLocationDetails;
using Fayora.Contracts.TouristModule;
using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Common.Mapper;

public class TouristMapper : Profile
{
    public TouristMapper()
    {
        CreateMap<MasterInterest, InterestDto>();
        CreateMap<GetInterestsResult, InterestsResponse>()
            .ForMember(dest => dest.Interests,
               opt => opt.MapFrom(src => src.Interests));

        CreateMap<CreateTouristProfileResult, CreateTouristResponse>();


        CreateMap<GetAllLocationsResult, GetAllLocationsResponse>();
        CreateMap<LocationSummaryResult, LocationSummaryResponse>();

        CreateMap<GetActivePackagesResult, ActivePackagesResponse>();
        CreateMap<ActivePackageSummaryResult, ActivePackageSummaryResponse>();

        CreateMap<GetLocationDetailsResult, LocationDetailsResponse>();
        CreateMap<LocationPackageSummaryResult, LocationPackageSummaryResponse>();
    }
}
