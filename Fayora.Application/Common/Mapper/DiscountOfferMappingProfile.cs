using AutoMapper;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Entities.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Mapper
{
    public class DiscountOfferMappingProfile : Profile
    {
        public DiscountOfferMappingProfile()
        {
            CreateMap<DiscountOffer, DiscountOfferResponse>()
                .ConstructUsing(src => new DiscountOfferResponse(
                src.Id,
                src.TargetId,
                src.TargetType.ToString(),
                src.Title,
                src.Description,
                src.DiscountType.ToString(),
                src.DiscountValue,
                src.StartDate,
                src.EndDate,
                src.Status.ToString()
                ));

        }
    }

}
