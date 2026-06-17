using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.SharedModule.Commands.CreateDiscountOffer
{
    public record CreateDiscountOfferCommand(
        Guid TargetId,
        OfferTargetType TargetType,
        string Title,
        string? Description,
        DiscountType DiscountType,
        decimal DiscountValue,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate,
        int? UsageLimit = null) : ICommand<Result<string>>;
}
