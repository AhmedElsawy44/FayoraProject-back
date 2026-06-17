using Fayora.Application.Features.SharedModule.Commands.CancelDiscountOffer;
using Fayora.Application.Features.SharedModule.Commands.CreateDiscountOffer;
using Fayora.Application.Features.SharedModule.Queries.GetActiveOffersByTarget;
using Fayora.Application.Features.SharedModule.Queries.GetMyOffers;
using Fayora.Application.Features.SharedModule.Queries.ValidatePromoCode;
using Fayora.Contracts.SharedModule.Requests;
using Fayora.Domain.Enums.SharedModule;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountOfferController(ISender sender) : ApiController
    {

        [HttpPost]
        public async Task<IActionResult> CreateOffer(
            [FromBody] CreateDiscountOfferRequest request,
            CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<OfferTargetType>(request.TargetType, true, out var targetType))
                return BadRequest("Invalid target type. Valid values: HousingUnit, GuidePackage, TourGuide.");

            if (!Enum.TryParse<DiscountType>(request.DiscountType, true, out var discountType))
                return BadRequest("Invalid discount type. Valid values: Percentage, FixedAmount.");

            var command = new CreateDiscountOfferCommand(
                request.TargetId,
                targetType,
                request.Title,
                request.Description,
                discountType,
                request.DiscountValue,
                request.StartDate,
                request.EndDate);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                offerId => Ok(new { OfferId = offerId }),
                errors => Problem(errors)
            );
        }


        [HttpPatch("{offerId:guid}/cancel")]
        public async Task<IActionResult> CancelOffer(
            [FromRoute] Guid offerId,
            CancellationToken cancellationToken)
        {
            var command = new CancelDiscountOfferCommand(offerId);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => (IActionResult)Ok(new { Message = "Offer cancelled successfully." }),  
                errors => Problem(errors)
            );

        }


        [HttpGet("by-target")]
        public async Task<IActionResult> GetOffersByTarget(
            [FromQuery] Guid targetId,
            [FromQuery] string targetType,
            CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<OfferTargetType>(targetType, true, out var parsedTargetType))
                return BadRequest("Invalid target type. Valid values: HousingUnit, GuidePackage, TourGuide.");

            var query = new GetActiveOffersByTargetQuery(targetId, parsedTargetType);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                value => Ok(value),
                errors => Problem(errors)
            );
        }


        [HttpGet("validate")]
        public async Task<IActionResult> ValidatePromoCode(
            [FromQuery] string code,
            [FromQuery] Guid targetId,
            [FromQuery] string targetType,
            CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<OfferTargetType>(targetType, true, out var parsedTargetType))
                return BadRequest("Invalid target type. Valid values: HousingUnit, GuidePackage, TourGuide.");

            var query = new ValidatePromoCodeQuery(code, targetId, parsedTargetType);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                value => Ok(value),
                errors => Problem(errors)
            );
        }


        [HttpGet("my-offers")]
        public async Task<IActionResult> GetMyOffers(
            [FromQuery] string? status,
            CancellationToken cancellationToken)
        {
            var query = new GetMyOffersQuery(status);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                value => Ok(value),
                errors => Problem(errors)
            );
        }
    }
}
