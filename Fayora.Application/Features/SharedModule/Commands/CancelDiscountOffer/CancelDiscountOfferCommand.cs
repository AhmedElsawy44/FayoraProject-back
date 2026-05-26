using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.SharedModule.Commands.CancelDiscountOffer
{
    public record CancelDiscountOfferCommand(Guid OfferId) : ICommand<Result<Success>>;
}
