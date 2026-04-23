using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.CreateCompanyPackage;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Common.Mapper;

public class TourCompanyMappingProfile : Profile
{
    public TourCompanyMappingProfile()
    {
        CreateMap<CreateTourCompanyResult, CreateTourCompanyResponse>();
        //CreateMap<GetTourCompanyByIdResult, GetTourCompanyByIdResponse>();
        CreateMap<CreateTourCompanyResult, CreateCompanyPackageResponse>();
        CreateMap<TourTypeDto, TourType>();
        //CreateMap<GetCompanyPackagesResult, GetCompanyPackagesResponse>();
        //CreateMap<GetAllPackagesResult, GetAllPackagesResponse>();
        //CreateMap<PackageItemResult, PackageItemResponse>();
    }
}
