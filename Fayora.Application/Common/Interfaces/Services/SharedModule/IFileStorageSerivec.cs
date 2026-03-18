namespace Fayora.Application.Common.Interfaces.Services.SharedModule;

public interface IFileStorageService
{
    Task DeleteFileAsync(string mediaURL, CancellationToken cancellationToken);
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
}