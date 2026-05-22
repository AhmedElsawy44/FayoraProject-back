using AutoMapper;
using Fayora.Application.Features.BookingModule.Queries.GetBookingDetails;
using Fayora.Contracts.BookingModule.GetBookingDetails;

namespace Fayora.Application.Common.Mapper
{
    internal class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {

            CreateMap<GetBookingDetailsResult, BookingDetailsResponse>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.ToString()));
        }
    }
}
