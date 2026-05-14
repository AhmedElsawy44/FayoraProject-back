using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Commands.GenerateBookingQr
{
    public record GenerateBookingQrCommand(
        Guid BookingId
    ) : ICommand<Result<GenerateBookingQrResult>>;

}
