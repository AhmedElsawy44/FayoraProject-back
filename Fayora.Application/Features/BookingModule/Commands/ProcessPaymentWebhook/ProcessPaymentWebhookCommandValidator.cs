using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.BookingModule.Commands.ProcessPaymentWebhook
{
    public class ProcessPaymentWebhookCommandValidator : AbstractValidator<ProcessPaymentWebhookCommand>
    {
        public ProcessPaymentWebhookCommandValidator()
        {
        }
    }
}
