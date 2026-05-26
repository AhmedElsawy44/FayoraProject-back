using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Contracts.AdminModule.GetBookings;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetBookings;

public class GetBookingsQueryHandler(IAdminRepository adminRepository) : IQueryHandler<GetBookingsQuery, Result<List<GetBookingsResponse>>>
{
    public async Task<Result<List<GetBookingsResponse>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await adminRepository.GetBookingsAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchQuery,
            request.StatusFilter,
            request.PaymentFilter,
            request.ServiceTypeFilter,
            cancellationToken);

        return bookings;
    }
}
