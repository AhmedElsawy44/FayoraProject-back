using AutoMapper;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;
using Fayora.Contracts.AccommodationModule.Responses;

namespace Fayora.Application.Common.Mapper;

public class AccommodationMapper : Profile
{
    public AccommodationMapper()
    {
        CreateMap<CreateUnitOwnerOwnerResult, CreateUnitOwnerProfileResponse>();
    }
}
