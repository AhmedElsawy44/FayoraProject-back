using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;

namespace Fayora.Application.Common.Mapper;

public class TourGuideMappingProfile : Profile
{
    public TourGuideMappingProfile()
    {
        CreateMap<CreateTourGuideResult, CreateTourGuideResponse>();
    }
}
