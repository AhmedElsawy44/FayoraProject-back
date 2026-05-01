using FluentValidation;

namespace Fayora.Application.Features.ChatModule.Commands.DeleteMessage;

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty()
            .WithMessage("Message ID is required.");
    }
}
