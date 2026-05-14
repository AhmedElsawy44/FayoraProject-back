using FluentValidation;

namespace Fayora.Application.Features.BookingModule.Commands.ProcessPaymentWebhook
{
    public class ProcessPaymentWebhookCommandValidator : AbstractValidator<ProcessPaymentWebhookCommand>
    {
        public ProcessPaymentWebhookCommandValidator()
        {
        }
    }
}
