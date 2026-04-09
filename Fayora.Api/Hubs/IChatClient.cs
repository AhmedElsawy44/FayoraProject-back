namespace Fayora.Api.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(object payload);
    Task MessageSentSuccess(object payload);
    Task MessageUpdated(object payload);
    Task MessageUpdateSuccess(object payload);
    Task MessageDeleted(object payload);
    Task MessageDeleteSuccess(object payload);
    Task ReceiveError(object errors);
    Task MessagesSeen(object payload);
}
