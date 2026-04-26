using FluentValidation;

namespace Fayora.Application.Features.ChatModule.Commands.MarkChatAsRead;

public class MarkChatAsReadCommandValidator : AbstractValidator<MarkChatAsReadCommand>
{
    public MarkChatAsReadCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("Chat ID is required.");
    }
}
