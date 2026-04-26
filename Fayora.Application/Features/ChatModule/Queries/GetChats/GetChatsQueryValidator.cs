using FluentValidation;

namespace Fayora.Application.Features.ChatModule.Queries.GetChats;

public class GetChatsQueryValidator : AbstractValidator<GetChatsQuery>
{
    public GetChatsQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(50)
            .WithMessage("Limit cannot exceed 50 chats per request.");

        RuleFor(x => x.Cursor)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .When(x => x.Cursor.HasValue)
            .WithMessage("Cursor cannot be a date in the future.");
    }
}