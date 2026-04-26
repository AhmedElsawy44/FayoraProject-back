using FluentValidation;
using Fayora.Domain.Enums.ChatModule;

namespace Fayora.Application.Features.ChatModule.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ReceiverId)
            .NotEmpty()
            .WithMessage("ReceiverId is required.");

        RuleFor(x => x.ScopeId)
            .NotEmpty()
            .WithMessage("ScopeId is required.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Message content cannot be empty.");

        RuleFor(x => x.MessageType)
            .NotEmpty()
            .WithMessage("MessageType is required.")
            .IsEnumName(typeof(MessageType), caseSensitive: false)
            .WithMessage("Invalid Message Type. Allowed values are: Text, Image, File, Voice, Video.");

        RuleFor(x => x.ScopeType)
            .NotEmpty()
            .WithMessage("ScopeType is required.")
            .IsEnumName(typeof(ChatScopeType), caseSensitive: false)
            .WithMessage("Invalid Scope Type. Allowed values are: HousingUnit, TourPackage, etc.");
    }
}