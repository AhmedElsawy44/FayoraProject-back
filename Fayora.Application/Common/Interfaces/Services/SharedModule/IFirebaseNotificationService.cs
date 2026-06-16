namespace Fayora.Application.Common.Interfaces.Services.SharedModule;

public interface IFirebaseNotificationService
{
    Task<(int SuccessCount, int FailureCount)> SendBroadcastAsync(
        string title,
        string body,
        string? imageUrl,
        List<string> targetTokens,
        CancellationToken cancellationToken,
        Dictionary<string, string>? data = null);

    Task<bool> SendToTokenAsync(
        string token,
        string title,
        string body,
        string? imageUrl,
        CancellationToken cancellationToken,
        Dictionary<string, string>? data = null);
}
