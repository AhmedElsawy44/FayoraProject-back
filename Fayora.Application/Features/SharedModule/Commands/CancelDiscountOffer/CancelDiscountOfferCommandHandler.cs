using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.SharedModule.Commands.CancelDiscountOffer
{
    public class CancelDiscountOfferCommandHandler(
        IClientContextProvider clientContextProvider,
        IDiscountOfferRepository discountOfferRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CancelDiscountOfferCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            CancelDiscountOfferCommand request,
            CancellationToken cancellationToken)
        {
            var ownerId = clientContextProvider.GetContext().UserId;

            var offer = await discountOfferRepository.GetByIdAsync(request.OfferId, cancellationToken);

            if (offer is null)
                return DiscountOfferErrors.NotFound(request.OfferId);


            if (offer.OwnerId != ownerId)
                return DiscountOfferErrors.Unauthorized;

            var cancelResult = offer.Cancel();
            if (cancelResult.IsError) return cancelResult.Errors;

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
