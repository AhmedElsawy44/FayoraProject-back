using AutoMapper;
using Fayora.Application.Features.TourCompanyModule.Commands.CreateCompanyPackage;
using Fayora.Application.Features.TourCompanyModule.Commands.CreateTourCompany;
using Fayora.Application.Features.TourCompanyModule.Queries;
using Fayora.Contracts.TourCompanyModule.CreateCompanyPackage;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.GetTourCompanyById;
using Fayora.Domain.Enums.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Mapper
{
    public class TourCompanyMappingProfile : Profile
    {
        public TourCompanyMappingProfile()
        {
            CreateMap<CreateTourCompanyResult, CreateTourCompanyResponse>();
            CreateMap<GetTourCompanyByIdResult, GetTourCompanyByIdResponse>();
            CreateMap<CreateCompanyPackageResult, CreateCompanyPackageResponse>();
            CreateMap<TourTypeDto, TourType>();
        }
    }
}
