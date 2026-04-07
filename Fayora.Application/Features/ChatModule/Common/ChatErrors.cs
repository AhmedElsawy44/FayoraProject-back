using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ChatModule.Common;

public static class ChatErrors
{
    public static readonly Error InvalidMessageType = Error.Validation(
        code: "InvalidMessageType",
        description: "The provided message type is invalid."
    );

    public static readonly Error InvalidScopeType = Error.Validation(
        code: "InvalidScopeType",
        description: "The provided scope type is invalid."
    );

    public static readonly Error ChatNotFound = Error.Validation(
        code: "ChatNotFound",
        description: "The provided chat ID does not exist."
    );

    public static readonly Error ReceiverNotFound = Error.NotFound(
        code: "ReceiverNotFound",
        description: "The receiver user was not found."
    );
}
