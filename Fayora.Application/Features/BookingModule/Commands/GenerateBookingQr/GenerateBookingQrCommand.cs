using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.BookingModule.Commands.GenerateBookingQr
{
    public record GenerateBookingQrCommand(
        Guid BookingId
    ) : ICommand<Result<GenerateBookingQrResult>>;

}
