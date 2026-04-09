using FluentValidation;

namespace Fayora.Application.Features.ChatModule.Commands.UpdateMessage;

public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty()
            .WithMessage("Message ID is required.");

        RuleFor(x => x.NewContent)
            .NotEmpty()
            .WithMessage("Message content cannot be empty.")

            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithMessage("Message content cannot consist only of white spaces.")
            .MaximumLength(2000)
            .WithMessage("Message content cannot exceed 2000 characters.");
    }
}