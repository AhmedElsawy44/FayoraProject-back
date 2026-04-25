using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Contracts.TourGuideModule.CreateTourCompany;

namespace Fayora.Application.Common.Mapper;

public class TourCompanyMappingProfile : Profile
{
    public TourCompanyMappingProfile()
    {
        CreateMap<CreateTourCompanyResult, CreateTourCompanyResponse>();
        //CreateMap<GetTourCompanyByIdResult, GetTourCompanyByIdResponse>();
        //CreateMap<GetCompanyPackagesResult, GetCompanyPackagesResponse>();
        //CreateMap<GetAllPackagesResult, GetAllPackagesResponse>();
        //CreateMap<PackageItemResult, PackageItemResponse>();
    }
}
