using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackageAccommodation;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackageOccurrenceDetails;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackagePreview;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.GetMyPackages;
using Fayora.Contracts.TourGuideModule.GetPackageAccommodation;
using Fayora.Contracts.TourGuideModule.GetPackageDetails;
using Fayora.Contracts.TourGuideModule.GetPackageOccurrenceDetails;
using Fayora.Contracts.TourGuideModule.GetPackagePreview;
using Fayora.Contracts.TourGuideModule.GetTourGuideById;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Mapper;

public class TourGuideMappingProfile : Profile
{
    public TourGuideMappingProfile()
    {
        CreateMap<CreateTourGuideResult, CreateTourGuideResponse>();


        CreateMap<City, CityResponse>()
        .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.CenterCoordinates.Latitude))
        .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.CenterCoordinates.Longitude));


        CreateMap<PackagePreviewResult, PackagePreviewResponse>();
        CreateMap<PackageActivityResult, PackageActivityResponse>();
        CreateMap<GeoPointResult, GeoPointResponse>();
        CreateMap<GuideInfoResult, GuideInfoResponse>();


        CreateMap<PackageDetailsResult, PackageDetailsResponse>();
        CreateMap<OptionalActivity, OptionalActivityResponse>();
        CreateMap<PackageActivityDetailsResult, PackageActivityDetailsResponse>();
        CreateMap<GeoPointDetailsResult, GeoPointResponse>();
        CreateMap<GuideInfoDetailsResult, GuideInfoResponse>();
        CreateMap<PackageOccurrenceResult, PackageOccurrenceResponse>();


        CreateMap<GetMyPackagesResult, MyPackagesResponse>();
        CreateMap<PackageSummaryResult, PackageSummaryResponse>();

        CreateMap<PackageNightDetailsResult, PackageNightDetailsResponse>();
        CreateMap<PackageNightResult, PackageNightResponse>();
        CreateMap<GetPackageAccommodationResult, GetPackageAccommodationResponse>();

        CreateMap<GetPackageOccurrenceDetailsResult, PackageOccurrenceDetailsResponse>();
        CreateMap<OccurrenceAttendeeResult, OccurrenceAttendeeResponse>();
    }
}
