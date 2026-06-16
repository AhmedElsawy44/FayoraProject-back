using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.BookingModule.Commands.CancelBooking
{
    public record CancelBookingCommand(Guid BookingId) : ICommand<Result<Success>>;
}
