using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitCalendarBlock;

public record CreateUnitCalendarBlockCommand
(
    Guid UnitId,
    DateOnly StartDate,
    DateOnly EndDate,
    BlockReason BlockReason
) : ICommand<Result<Unit>>;
