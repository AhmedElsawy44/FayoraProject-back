using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.BookingModule.CreateGuideBooking
{
    public record CreateGuideBookingRequest(
        DateOnly BookingDate,
        TimeSpan StartTime,
        int Adults,
        int Children,
        string PaymentMethodType
    );
}
