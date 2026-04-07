using Fayora.Domain.Common.Results;

namespace Fayora.Domain.Errors;

public static class ChatErrors
{
    public static readonly Error SelfChatNotAllowed = Error.Validation(
        "Chat.SelfChatNotAllowed",
        "User cannot start a chat with themselves."
    );

    public static readonly Error EmptyMessage = Error.Validation(
        "Chat.EmptyMessage",
        "Message content cannot be empty."
    );

    public static readonly Error NonTextMessageCannotBeEdited = Error.Validation(
        "Chat.NonTextMessageCannotBeEdited",
        "Only text messages can be edited."
    );

    public static readonly Error MessageEditTimeExpired = Error.Validation(
        "Chat.MessageEditTimeExpired",
        "Message cannot be edited after 1 hour of sending."
    );
}
