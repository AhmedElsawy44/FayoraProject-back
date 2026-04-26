using FluentValidation;

namespace Fayora.Application.Features.ChatModule.Queries.GetMessages;

public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
{
    public GetMessagesQueryValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("Chat Id is required.");


        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Limit cannot exceed 100 messages per request.");

        RuleFor(x => x.Cursor)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .When(x => x.Cursor.HasValue)
            .WithMessage("Cursor cannot be a date in the future.");
    }
}