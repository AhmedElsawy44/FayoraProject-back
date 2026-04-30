using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Enums.BookingModule;

public enum PaymentTransactionStatus
{
    Pending,
    Success,
    Failed,
    Refunded
}
