using AutoMapper;
using Fayora.Application.Features.TourCompanyModule.Commands.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
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
        }
    }
}
