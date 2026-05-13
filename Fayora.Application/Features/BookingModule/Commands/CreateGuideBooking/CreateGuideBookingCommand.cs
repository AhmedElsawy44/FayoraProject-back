using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.BookingModule.Commands.CreateGuideBooking
{
    public record CreateGuideBookingCommand(
        Guid GuideId,
        DateOnly BookingDate,
        TimeSpan StartTime,
        int Adults,
        int Children,
        PaymentMethodType PaymentMethodType
    ) : ICommand<Result<string>>;
}
