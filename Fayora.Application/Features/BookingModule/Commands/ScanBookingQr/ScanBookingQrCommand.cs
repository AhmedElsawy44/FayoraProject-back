using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Commands.ScanBookingQr
{
    public record ScanBookingQrCommand(
        string Token
    ) : ICommand<Result<ScanBookingQrResult>>;
}
