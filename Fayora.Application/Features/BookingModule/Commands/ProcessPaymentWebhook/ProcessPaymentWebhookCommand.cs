using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.BookingModule.Commands.ProcessPaymentWebhook;

public record ProcessPaymentWebhookCommand(
    string JsonPayload,
    string ReceivedHmac
    ) : ICommand<Result<Unit>>;
