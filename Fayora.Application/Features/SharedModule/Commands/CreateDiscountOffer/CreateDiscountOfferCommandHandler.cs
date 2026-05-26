using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.SharedModule.Commands.CreateDiscountOffer
{
    public class CreateDiscountOfferCommandHandler(
        IClientContextProvider clientContextProvider,
        IDiscountOfferRepository discountOfferRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateDiscountOfferCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            CreateDiscountOfferCommand request,
            CancellationToken cancellationToken)
        {
            var ownerId = clientContextProvider.GetContext().UserId;

            // check that this target dont have active offers
            var hasActive = await discountOfferRepository.HasActiveOfferForTargetAsync(
                request.TargetId,
                request.TargetType,
                cancellationToken);

            if (hasActive)
                return DiscountOfferErrors.ActiveOfferAlreadyExists;

            var offerResult = DiscountOffer.Create(
                ownerId,
                request.TargetId,
                request.TargetType,
                request.Title,
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.StartDate,
                request.EndDate);

            if (offerResult.IsError) return offerResult.Errors;

            discountOfferRepository.Add(offerResult.Value);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return offerResult.Value.Id.ToString();
        }
    }
}
