namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IDailyUploadTracker
{
    Task<int> GetTodayUploadCountAsync(Guid userId, UploadContext context, CancellationToken cancellationToken);

    Task IncrementUploadCountAsync(Guid userId, UploadContext context, int count, CancellationToken cancellationToken);
}
