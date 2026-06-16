using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.SharedModule.Responses;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.SharedModule.Queries.GetMyOffers
{
    public record GetMyOffersQuery(string? Status) : IQuery<Result<List<DiscountOfferResponse>>>;
}
