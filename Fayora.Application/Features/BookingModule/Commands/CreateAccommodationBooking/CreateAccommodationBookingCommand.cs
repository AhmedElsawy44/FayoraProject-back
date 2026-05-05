using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.BookingModule.Commands.CreateAccommodationBooking
{
    public record CreateAccommodationBookingCommand(
        Guid UnitId,
        DateOnly StartDate,
        DateOnly EndDate,
        int Adults,
        int Children,
        PaymentMethodType PaymentMethodType
    ) : ICommand<Result<string>>;
}
