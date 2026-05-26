using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Queries.GetBookingDetails
{
    public record GetBookingDetailsQuery(Guid BookingId) : IQuery<Result<GetBookingDetailsResult>>;
}
