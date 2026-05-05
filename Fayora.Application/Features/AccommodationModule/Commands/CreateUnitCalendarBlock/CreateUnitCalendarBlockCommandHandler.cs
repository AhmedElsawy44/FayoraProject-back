using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Enums.BookingModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitCalendarBlock;

public class CreateUnitCalendarBlockCommandHandler(
    ICalendarBlockRepository calendarBlockRepository,
    IHousingUnitRepository housingUnitRepository,
    IClientContextProvider clientContextProvider,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CreateUnitCalendarBlockCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(CreateUnitCalendarBlockCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var unit = await housingUnitRepository.GetUnitByIdAsync(request.UnitId, new UnitQueryOptions { IsReadOnly = true }, cancellationToken);

        if (unit is null || unit.OwnerId != userId) return AccommodationErrors.UnitNotFound;

        var startDateTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = request.EndDate.ToDateTime(TimeOnly.MinValue);

        if (await bookingRepository.HasOverlapAsync(unit.Id, startDateTime, endDateTime, cancellationToken))
            return AccommodationErrors.UnitAlreadyBlockedOrBooked;

        var block = new CalendarBlock(unit.Id, ServiceType.Accommodation, startDateTime, endDateTime, request.BlockReason);

        calendarBlockRepository.AddCalendarBlock(block);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
