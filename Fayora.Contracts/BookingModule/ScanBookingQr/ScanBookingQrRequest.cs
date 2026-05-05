using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.BookingModule.ScanBookingQr
{
    public record ScanBookingQrRequest(
        string Token
    );
}
