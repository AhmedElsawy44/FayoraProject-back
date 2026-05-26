using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.BookingModule.Commands.ConfirmCashReceived
{
    public record ConfirmCashReceivedCommand(Guid BookingId) : ICommand<Result<Success>>;
}
